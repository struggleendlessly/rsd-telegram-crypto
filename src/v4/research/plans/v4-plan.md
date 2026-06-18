# v4 — Solana Memecoin Discovery System (F# backend + Blazor UI)

## Context

You want a new application under [src/v4/](../../) (currently empty) that continuously discovers **all new SPL token mints** on Solana, enriches them (supply, metadata, price, market cap), and stores them in **Postgres** with both the **encoded (base58) and decoded (raw 32‑byte) contract address**. A web UI must show discovery statistics (today / last hour / total), per‑coin pricing (market cap, price in USDC and/or SOL — including, for SOL‑only coins, both the *current* and *at‑launch* SOL→USDC value plus the supply at that time), a coverage timeline from Solana genesis → now, and a searchable, expandable coin list.

Because the data volume is colossal, ingestion must be **heavily multithreaded and CPU/memory‑optimized**. The backend/API is **fully functional F#**; the frontend is **C# Blazor (SSR + WASM `InteractiveAuto`)** calling the F# API over HTTP/JSON only.

The repo already has a mature .NET 8/9 F# ecosystem (module‑per‑file functional style, Options pattern, Serilog→OpenTelemetry, Polly, batched JSON‑RPC, `BackgroundService` workers). v4 **reuses these conventions** but deliberately diverges from the legacy stack on two points: **Postgres instead of SQL Server**, and **no EF Core** (functional Npgsql + Dapper.FSharp + binary `COPY`). Other `src/*` apps are reference‑only — not modified.

## Decisions locked in (from clarification)

1. **Coin scope** — detect **all new SPL mints** chain‑wide (`spl-token` + `spl-token-2022`, `initializeMint` / `initializeMint2`), with a tunable filtering stage to cut noise.
2. **Node access** — pluggable block source; primary is **direct same‑server access via a Geyser/Yellowstone gRPC stream** (real low‑overhead path — reading the validator's RocksDB ledger directly is impractical/unsafe while it runs), with **local JSON‑RPC `getBlock`** for backfill and **remote RPC (Alchemy/Helius)** as configured failover. A Geyser→Postgres plugin is a viable alternative DB feeder if you prefer.
3. **Time range** — **forward/live** scanner (now → onward) **plus** a configurable **N‑days backfill** scanner; both resumable and idempotent. UI shows a **coverage timeline** (genesis slot 0 → current tip) of scanned vs unscanned ranges.
4. **UI** — **C# Blazor Web App, `InteractiveAuto` (SSR + WASM)**; **all API in F#**. Language boundary is HTTP/JSON only (no cross‑language project references).

## Architecture — projects under `src/v4`

```
src/v4/
  v4.Domain/        (F# lib)   pure types, pricing math, base58, Options classes — no IO
  v4.Blocks/        (F# lib)   IBlockSource + Geyser/LocalRpc/RemoteRpc/Composite, streaming MintScanner
  v4.Persistence/   (F# lib)   NpgsqlDataSource, Dapper.FSharp repos, binary COPY, migrations runner, coverage, bloom dedup
  v4.Enrich/        (F# lib)   supply / metadata / price enrichment
  v4.Ingestion/     (F# worker, Sdk.Worker)  Channels pipeline + Forward & Backfill hosted services
  v4.Api/           (F# Sdk.Web)  Minimal API (stats, coins, coverage, sol-usd)
  v4.Web.Shared/    (C# classlib) DTO records + typed CoinApiClient
  v4.Web/           (C# Sdk.Web)  Blazor host, InteractiveServer + InteractiveWebAssembly
  v4.Web.Client/    (C# Sdk.BlazorWebAssembly) WASM components
  v4.sln
```

**Dependency graph (acyclic):**
- `v4.Blocks → v4.Domain`; `v4.Persistence → v4.Domain`; `v4.Enrich → v4.Domain, v4.Blocks`
- `v4.Ingestion → v4.Domain, v4.Blocks, v4.Persistence, v4.Enrich`
- `v4.Api → v4.Domain, v4.Persistence`
- `v4.Web.Client → v4.Web.Shared`; `v4.Web → v4.Web.Shared, v4.Web.Client`
- F# (Api/Ingestion) and C# (Web) communicate **only over HTTP/JSON**.

F# projects use the repo's `module-per-file` + explicit `<Compile Include>` ordering. `v4.Domain` is the dependency root (zero project refs), mirroring `src/f#/shared/shared.fsproj`.

## Ingestion pipeline (multithreaded, memory‑bounded)

Use **`System.Threading.Channels`** (bounded → backpressure) to form a fixed stage graph; bounded capacity is the memory governor (slow persist → backpressure all the way to fetch). CPU‑bound stages get `Environment.ProcessorCount` workers; IO‑bound stages get larger pools tuned to provider throughput.

```
[Slot Planner] → chan<SlotRange>
  → [Fetch]   (IBlockSource, N workers)        → chan<RawBlock>     (small cap)
  → [Parse]   (Utf8JsonReader, M=cores)         → chan<ParsedBlock>  (bounded)
  → [Detect]  (mint scan, partitioned, cores)   → chan<MintEvent>    (large cap — tiny structs)
  → [Filter]  (in‑proc bloom + DB probe)         → chan<MintEvent>
  → [Enrich]  (batched RPC, K workers)           → chan<EnrichedMint>
  → [Persist] (batch accumulator → COPY)         → Postgres
```

Stage signature shape: `ChannelReader<'in> -> ChannelWriter<'out> -> CancellationToken -> Task`; `Pipeline.run` wires channels with explicit caps, fans out workers via `Array.init`, awaits `Task.WhenAll`, and propagates `writer.Complete()` for clean drain on shutdown.

**Memory/CPU optimizations (the "colossal volume" requirement):**
- **Streaming detect, not full deserialize.** A dedicated `MintScanner.scan : ReadOnlySpan<byte> -> MintEvent[]` walks the block JSON with `Utf8JsonReader` and `reader.Skip()`s irrelevant transactions/subtrees — it never materializes the full block object graph (that allocation cost is what the legacy swaps path pays). Template: the custom converter style in `src/f#/apiClients/rpc/alchemy/sol/DTO/response/responseGetBlock.fs`.
- **`ArrayPool<byte>.Shared`** for raw block buffers (Fetch rents, Parse returns); RPC payloads are never turned into `string`.
- **`[<Struct>]` records + `voption`** in the hot detect path (millions of events) to avoid heap/box allocations.
- **Base58 decode once** per mint → store both `mint_b58` (string) and `mint_bytes` (32‑byte `bytea`).
- **Batched binary `COPY`** via Npgsql `BeginBinaryImport` (`COPY ... FROM STDIN (FORMAT BINARY)`), flushing on row‑count or time. COPY can't upsert, so COPY into a temp/staging table then `INSERT ... SELECT ... ON CONFLICT (mint_b58) DO NOTHING`.
- **No shared mutable state** across parse/detect workers (each owns its scanner state) → lock‑free, saturates cores.

## Block sources & scan coordination

`IBlockSource` abstraction (`GetTipSlot`, `FetchRange : SlotRange -> IAsyncEnumerable<RawBlock>`, `Health`), implementations:
- **GeyserBlockSource** (live primary): Yellowstone gRPC on the same server. No official .NET client → generate one with `Grpc.Tools` from the public `geyser.proto` (`Grpc.Net.Client` + `Google.Protobuf`); F# consumes the generated client. **Spike this first** — highest‑uncertainty piece (proto/plugin version must match the validator).
- **LocalRpcBlockSource** (backfill primary): `getBlock` against the local validator, reusing the batching shape from `src/f#/apiClients/rpc/alchemy/sol/ApiCallerSOL.fs` (`chunkBySize 20 >> Array.mapi makeRequest >> Async.Parallel`) and builders in `src/f#/apiClients/rpc/alchemy/sol/UlrBuilderSOL.fs`.
- **RemoteRpcBlockSource** (failover): Alchemy/Helius via the existing multi‑key + Polly pattern.
- **CompositeBlockSource**: ordered failover per mode, with a background probe re‑promoting the primary when healthy. Config via a new `BlockSourceOption` following `src/f#/shared/Options/ChainSettingsOption.fs` (`SectionName`, mutable members).

**Two concurrent hosted services** (clone the `IScopedProcessingService` + `BackgroundService` + `IDictionary<string,IScopedProcessingService>` wiring from `src/f#/solana/wsSwaps/Program.fs`):
- **Forward**: plans `[max(coverage_tip, GetTipSlot - lag) … tip]`, Geyser first.
- **Backfill**: walks backward in fixed chunks from an anchor down to `tip − BackfillDays`, LocalRpc first.

**Idempotency / resume:** `scan_coverage` records contiguous processed slot ranges; the planner reads it to skip covered ranges and resume after restart. The Persist stage commits the **coverage row in the same transaction as the COPY batch** → re‑processing a range on crash is safe. Generalize the existing slot‑range generator `src/f#/shared/logic/bl_createSeq.fs` (`getSeqToProcessUint64`) into the bidirectional planner.

## Postgres schema (plain SQL migrations, applied at startup)

Use `numeric` for all money/amounts (never `double` — the legacy `swapsTokens` used `double`; do not repeat for stored prices), `bytea` for decoded pubkeys, `timestamptz`, `bigint` for slots.

- **`coins`** — `id`, `mint_b58 text`, `mint_bytes bytea`, `name_long`, `name_short`, `decimals`, `discovered_slot`, `discovered_at`, **`launch_sol_usd numeric`**, **`launch_supply numeric`**, timestamps. `UNIQUE(mint_b58)`, `UNIQUE(mint_bytes)`. **BRIN** on `discovered_at` (append‑only/time‑ordered → cheap today/hour/total). `pg_trgm` GIN on names for search.
- **`coin_price_snapshots`** — `coin_id fk`, `observed_slot`, `observed_at`, `price_in_sol`, `price_in_usdc`, `sol_usd_at_obs`, `circulating_supply`, `market_cap_usd`, `source ('direct_usdc'|'derived_from_sol')`. `UNIQUE(coin_id, observed_slot)`.
- **`sol_usd_ref`** — `slot pk`, `observed_at`, `sol_usd numeric`. "Current" = newest row.
- **`scan_coverage`** — `slot_from`, `slot_to`, `time_from`, `time_to`; non‑overlap enforced by `btree_gist` exclusion on `int8range(slot_from, slot_to, '[]')`. **Merge‑on‑write** (coalesce adjacent/overlapping segments on insert).

**Launch‑value requirement** is satisfied by `coins.launch_sol_usd` + `coins.launch_supply` (captured once at discovery), with current values from the newest `sol_usd_ref` / `coin_price_snapshots` rows — so a SOL‑only coin shows both launch and current USD with no join gymnastics.

**Stats:** start with direct BRIN range queries (`count(*) where discovered_at >= …`); add a small counter/rollup table only if profiling demands it. **No materialized view** (a "last hour" view would need constant refresh).

Data access: **Dapper.FSharp** over a singleton `NpgsqlDataSource`; raw `Dapper` for the analytic coverage/gap query and aggregates.

## Pricing math

Port the existing functional pricing from `src/f#/solana/wsSwaps/mappers/mapGetSwaps.fs` into `v4.Domain`: `priceTokenInSol = solAmount / tokenAmount`, `priceSolInUsd = stableAmount / solAmount`, averaging. Market cap = `price_in_usdc × circulating_supply`. Maintain the `sol_usd_ref` series so SOL‑only coins can report USD at any slot.

## F# API (`v4.Api`) — ASP.NET Minimal API in F#

Matches the existing `XTwitter.F#/Program.fs` `WebApplication` + `MapGet` style (Giraffe adds no payoff for a few read endpoints). DTOs are F# records (camelCase JSON, `decimal` for money, decoded pubkey transported as hex).

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/stats` | today / hour / total + current sol_usd |
| GET | `/api/coins?search=&page=&pageSize=&sort=` | paged list / search |
| GET | `/api/coins/{mint}` | full detail (coin + launch ref + latest snapshot + current sol_usd) |
| GET | `/api/coins/{mint}/prices?from=&to=` | snapshot history |
| GET | `/api/coverage` | merged segments + computed gaps genesis→tip |
| GET | `/api/sol-usd/current` | latest sol_usd |
| GET | `/health` | liveness |

Migrations run at startup; `AddCors` for the Blazor origin. Reuse Serilog→OpenTelemetry/Options blocks from `src/f#/solana/wsSwaps/Program.fs`.

**Coverage gaps query:** order `scan_coverage` by `slot_from`, use `lead()` to synthesize gap rows where `next_from > slot_to + 1`; API prepends a leading gap from slot 0 and appends a trailing gap to the current tip.

## Blazor Web App (`v4.Web` + `v4.Web.Client` + `v4.Web.Shared`)

New Blazor Web App with `InteractiveAuto` (vs the Server‑only `src/BlazorApp`): server registers `AddInteractiveServerComponents()` + `AddInteractiveWebAssemblyComponents()`; `App.razor` uses `@rendermode="InteractiveAuto"`. A typed `CoinApiClient` (HttpClient → F# API base URL) lives in `v4.Web.Shared`, referenced by both host and WASM client so the same client/DTOs run during SSR and after WASM activation (hence CORS on the API).

Pages/components:
- `Dashboard.razor` — Today / Last hour / Total cards + current vs launch SOL/USD context + `CoverageTimeline`.
- `CoverageTimeline.razor` — proportional bar genesis→tip, green = scanned / gray = gap, tooltips with slot+time ranges (optionally log‑scaled).
- `Coins.razor` — searchable, paged, debounced table; rows expand (`CoinRow.razor`) to show market cap, price‑in‑USDC, price‑in‑SOL, and for SOL‑only coins both launch sol_usd + launch supply and current sol_usd. Detail lazy‑loaded on expand.
- Layout cloned from existing `BlazorApp/Components/Layout`.

## Key risks

- **Geyser gRPC .NET client** is the biggest unknown — spike before committing the live path.
- **Block size at scale** — the streaming `MintScanner` (skip‑subtree) is essential; if the local node only cheaply serves `encoding=base64`, fall back to Solnet binary instruction decoding instead of `parsed.type`.
- **COPY can't upsert** — stage‑then‑`ON CONFLICT`; bloom + `UNIQUE` carry idempotency; coverage commit shares the COPY transaction.
- **Channel cap tuning** — too high → OOM, too low → idle cores; load‑test.
- **Forward/backfill overlap** — both write `coins`; rely on DB unique + coverage ranges, never assume disjoint ownership.
- **Token‑2022** metadata/extensions differ from legacy SPL — detect handles both program IDs.

## NuGet (versions to mirror from existing `wsSwaps.fsproj`)

`Npgsql`, `Dapper`, `Dapper.FSharp`; `Grpc.Net.Client` + `Google.Protobuf` + `Grpc.Tools` (Geyser); `SimpleBase` or `Solnet.Wallet` (base58); `Solnet.Programs` (optional binary decode fallback); `Serilog(.Sinks.OpenTelemetry/.Console/.Formatting.Compact)`, `Microsoft.Extensions.Hosting(.WindowsServices)`, `Microsoft.Extensions.Http.Polly`.

## Build sequence

1. (Done) Plan saved to `src/v4/research/plans/v4-plan.md`.
2. Create `v4.sln` + 9 projects; F# `<Compile Include>` ordered bottom‑up (Domain → Blocks/Persistence → Enrich → Ingestion/Api).
3. Add NuGet refs; wire `Grpc.Tools` `<Protobuf Include="geyser.proto">` codegen in `v4.Blocks`. `dotnet build` to confirm codegen + compile order.
4. Spike `GeyserBlockSource` against the local validator (stream + proto compatibility).
5. Author Postgres DDL migrations; run `v4.Api` → migrations apply → `GET /health`, `GET /api/stats` (zeros).
6. Run `v4.Ingestion` with a short backfill window (a few hours) first; verify COPY throughput, coverage commit, idempotent restart (kill mid‑batch → no dupes / no gaps).
7. Scale channel caps + worker counts to saturate CPU; enable forward + backfill concurrently.
8. Run `v4.Web` (set `ApiBaseUrl`) → Dashboard, search, row‑expand.

## Verification (end‑to‑end)

- **Unit:** pure functions in `v4.Domain` (base58 round‑trip encode/decode = identity; pricing math; coverage gap synthesis on sample segment sets).
- **Detect:** feed a saved block JSON containing a known `initializeMint` → assert the expected `MintEvent` (the legacy code's commented `File.ReadAllText` of a sample block is a precedent for fixture‑driven testing).
- **Idempotency:** run the same slot range twice → row counts unchanged; kill the worker mid‑COPY → restart → no duplicate `mint_b58`, no coverage gaps.
- **DB:** after a backfill window, `coins`, `coin_price_snapshots`, `sol_usd_ref`, `scan_coverage` populate; `/api/stats` counts match SQL `count(*)`.
- **API:** `curl` each endpoint; `/api/coverage` returns alternating scanned/gap segments covering 0→tip.
- **UI:** Dashboard SSR first paint then upgrades to interactive; CORS fetch from WASM succeeds (DevTools); search + expand show market cap, USDC/SOL price, and launch‑vs‑current SOL→USDC for SOL‑only coins.
- **Perf:** observe CPU saturation across cores and bounded memory under load via the Serilog→OpenTelemetry sink during a sustained backfill.

## Open points (non‑blocking — default chosen, confirm during build)

- Target framework: **net9.0** across v4 (consistency with the F# side); the existing BlazorApp is net8 — Blazor net9 is fine.
- Stats rollup table: **deferred** (add only if profiling shows direct BRIN queries are too slow).
- "Current tip" source for the trailing coverage gap: **worker‑reported tip** (fallback: max `sol_usd_ref.slot`).
