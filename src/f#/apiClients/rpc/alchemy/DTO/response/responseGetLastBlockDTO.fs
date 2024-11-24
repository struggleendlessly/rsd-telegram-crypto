module responseGetLastBlockDTO

open responseErrorDTO

type responseGetLastBlockDTO = {
    jsonrpc: string
    id: int
    result: string
    error: responseErrorDTO
}

type responseGetLastBlocksDTO = responseGetLastBlockDTO[]
