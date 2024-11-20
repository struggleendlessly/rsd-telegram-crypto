module requestBaseDTO

open System.Text.Json.Serialization

type requestBaseDTO = {
    jsonrpc: string
    method: string
    [<JsonPropertyName("params")>]
    _params: obj[]
    id: int
}
with
    static member Default = 
         {
              jsonrpc = "2.0"
              method = "eth_getBlockByNumber"
              _params = [||]
              id = 0
          }

