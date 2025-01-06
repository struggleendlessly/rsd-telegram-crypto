module responseGetBlockSol

open System
open responseError

open System.Text.Json
open System.Text.Json.Serialization

type VersionConverter() =
    inherit JsonConverter<string>()

    override this.Read(reader: byref<Utf8JsonReader>, _, _ ) =
        match reader.TokenType with
        | JsonTokenType.String -> reader.GetString()
        | JsonTokenType.Number -> reader.GetInt32().ToString()
        | _ -> raise (JsonException("Unexpected token type"))

    override this.Write(writer: Utf8JsonWriter, value: string, _ ) =
        writer.WriteStringValue(value)

type UiTokenAmount = {
    amount: string
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
    destination: string option
    tokenAmount: TokenAmount option
}

and TokenAmount = {
    amount: string
    decimals: uint64
    uiAmount: float
    uiAmountString: string
}

type Parsed = {
    info: ParsedInfo
    ``type``: string
}

type Instruction = {
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
    [<JsonConverter(typeof<VersionConverter>)>]
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
    [<JsonConverter(typeof<VersionConverter>)>]
    id: string
    result: BlockResult 
    error: responseError option
}
