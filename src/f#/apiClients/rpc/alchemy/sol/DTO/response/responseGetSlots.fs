module responseGetSlots

open System
open responseError

type responseGetSlots = {
    jsonrpc: string
    id: string
    result: UInt64
    error: responseError option
}