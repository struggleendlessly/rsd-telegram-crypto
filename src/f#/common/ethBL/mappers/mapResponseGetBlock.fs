module mapResponseGetBlock

open System

open responseGetBlock
open Extensions
open ethCommonDB.models

let map (responseGetBlocksDTO: responseGetBlock) = 
    let res = new BlocksEntity()
    let block = responseGetBlocksDTO.result

    res.numberInt <- block.number.ToInt()
    res.numberHex <- block.number

    res.timestampUnix <- block.timestamp
    res.timestampNormal <-  DateTimeOffset.FromUnixTimeSeconds(block.timestamp.HexToInt64()).UtcDateTime 

    res.baseFeePerGas <- block.baseFeePerGas
    res.gasLimit <- block.gasLimit
    res.gasUsed <- block.gasUsed

    res.hash <- block.hash

    res

let mapBlocks =          
        Array.collect id
        >> Array.Parallel.map map             
