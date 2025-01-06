module responseGetBlockSol

open System
open responseError

open System.Text.Json
open System.Text.Json.Serialization

type converterNumberToString() =
    inherit JsonConverter<string>()

    override this.Read(reader: byref<Utf8JsonReader>, _, _ ) =
        match reader.TokenType with
        | JsonTokenType.String -> reader.GetString()
        | JsonTokenType.Number -> reader.GetInt32().ToString()
        | _ -> raise (JsonException("Unexpected token type"))

    override this.Write(writer: Utf8JsonWriter, value: string, _ ) =
        writer.WriteStringValue(value)

type ConverterStringToUint64() =
    inherit JsonConverter<uint64>()

    override this.Read(reader: byref<Utf8JsonReader>, _, _) =
        match reader.TokenType with
        | JsonTokenType.String ->
            let str = reader.GetString()
            match System.UInt64.TryParse(str) with
            | true, value -> value
            | false, _ -> raise (JsonException("Invalid uint64 value"))
        | _ -> raise (JsonException("Unexpected token type"))

    override this.Write(writer: Utf8JsonWriter, value: uint64, _) =
        writer.WriteStringValue(value.ToString())

// Usage Example
let options = JsonSerializerOptions()
options.Converters.Add(ConverterStringToUint64())

let json = """ "12345678901234567890" """
let value = JsonSerializer.Deserialize<uint64>(json, options)
printfn "Deserialized value: %A" value

type UiTokenAmount = {
    [<JsonConverter(typeof<ConverterStringToUint64>)>]
    amount: uint64
    decimals: uint64
    uiAmount: float option
    uiAmountString: string
}

type TokenBalance = {
    accountIndex: uint64
    mint: string
    owner: string
    programId: string
    uiTokenAmount: UiTokenAmount
}

type Status = {
    Ok: obj option
}

type InnerInstructionDetail = {
    lamports: uint64 option
    newAccount: string option
    owner: string option
    source: string option
    space: uint64 option
    account: string option
    mint: string option
    authority: string option
    destination: string option
    tokenAmount: UiTokenAmount option
    extensionTypes: string list option
    systemProgram: string option
    tokenProgram: string option
    wallet: string option
}

type InnerInstructionParsed = {
    info: InnerInstructionDetail
    ``type``: string
}

type ParsedInfo = {
    lamports: uint64 option
    newAccount: string option
    owner: string option
    source: string option
    space: uint64 option
    account: string option
    mint: string option
    systemProgram: string option
    tokenProgram: string option
    wallet: string option
    extensionTypes: string[] option
    authority: string option
    [<JsonConverter(typeof<ConverterStringToUint64>)>]
    amount: uint64
    destination: string option
    tokenAmount: TokenAmount option
}

and TokenAmount = {
    [<JsonConverter(typeof<ConverterStringToUint64>)>]
    amount: uint64
    decimals: int
    uiAmount: float
    uiAmountString: string
}
type Parsed = {
    info: ParsedInfo   
    ``type``: string
}

type ParsedConverter() =
    inherit JsonConverter<Parsed option>()

    override this.Read(reader: byref<Utf8JsonReader>, t: Type, options: JsonSerializerOptions) =
        match reader.TokenType with
        | JsonTokenType.String ->
            let str = reader.GetString()
            None
        | JsonTokenType.StartObject ->
            let obj = JsonSerializer.Deserialize<Parsed>(&reader, options)
            Some obj

    override this.Write(writer: Utf8JsonWriter, value: Parsed option, _) =
        writer.WriteStringValue(value.ToString())

type Instruction = {
    [<JsonConverter(typeof<ParsedConverter>)>]
    parsed: Parsed option
    program: string
    programId: string
    stackHeight: uint64
    accounts: string[] option
    data: string option
}

type InnerInstruction = {
    index: uint64
    instructions: Instruction[]
}


type Meta = {
    computeUnitsConsumed: uint64
    err: obj option
    fee: uint64
    innerInstructions: InnerInstruction list
    logMessages: string list
    postBalances: int64 list
    postTokenBalances: TokenBalance list
    preBalances: int64 list
    preTokenBalances: TokenBalance list
    rewards: obj option
    status: Status
}

type AccountKey = {
    pubkey: string
    signer: bool
    source: string
    writable: bool
}

type Message = {
    accountKeys: AccountKey list
    instructions: InnerInstruction list
    recentBlockhash: string
}

type Transaction = {
    message: Message
    signatures: string list
}

type TransactionData = {
    meta: Meta
    transaction: Transaction
    [<JsonConverter(typeof<converterNumberToString>)>]
    version: string
}

type BlockResult = {
    blockHeight: uint64
    blockTime: int64
    blockhash: string
    parentSlot: uint64
    previousBlockhash: string
    transactions: TransactionData list
}

type responseGetBlockSol = {
    jsonrpc: string
    [<JsonConverter(typeof<converterNumberToString>)>]
    id: string
    result: BlockResult 
    error: responseError option
}
