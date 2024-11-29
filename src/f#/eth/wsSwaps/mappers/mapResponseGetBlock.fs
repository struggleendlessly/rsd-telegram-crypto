module mapResponseGetBlock

open System
open dbMigration.models
open responseGetBlockDTO
open Extensions

let map (responseGetBlocksDTO: responseGetBlockDTO) = 
    let res = new EthBlocksEntity()
    let block = responseGetBlocksDTO.result

    res.numberInt <- block.number.ToInt()
    res.numberHex <- block.number

    res.timestampUnix <- block.timestamp
    res.timestampNormal <-  DateTimeOffset.FromUnixTimeSeconds(block.timestamp.HexToInt64()).UtcDateTime 

    res.baseFeePerGas <- block.baseFeePerGas
    res.gasLimit <- block.gasLimit
    res.gasUsed <- block.gasUsed

    res