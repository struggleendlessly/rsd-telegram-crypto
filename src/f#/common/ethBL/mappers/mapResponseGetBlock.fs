module mapResponseGetBlock

open System

open responseGetBlock
open Extensions
open ethCommonDB.models

let map (responseGetBlocksDTO: responseGetBlock.Result) = 
    let res = new BlocksEntity()
    //let block = responseGetBlocksDTO.result

    res.numberInt <- responseGetBlocksDTO.number.ToInt()
    res.numberHex <- responseGetBlocksDTO.number

    res.timestampUnix <- responseGetBlocksDTO.timestamp
    res.timestampNormal <-  DateTimeOffset.FromUnixTimeSeconds(responseGetBlocksDTO.timestamp.HexToInt64()).UtcDateTime 

    res.baseFeePerGas <- responseGetBlocksDTO.baseFeePerGas
    res.gasLimit <- responseGetBlocksDTO.gasLimit
    res.gasUsed <- responseGetBlocksDTO.gasUsed

    res.hash <- responseGetBlocksDTO.hash

    res

let mapBlocks () =   
        Seq.collect id
        >> Seq.choose (fun x -> x.result)
        >> Seq.map map             