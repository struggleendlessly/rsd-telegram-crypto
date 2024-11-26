module mapResponseGetBlock

open System
open dbMigration.models
open responseGetBlockDTO
open Extensions

let map responseGetBlocksDTO = 
    let res = new EthBlocksEntity()

    res.numberInt <- responseGetBlocksDTO.number.ToInt()
    res.numberHex <- responseGetBlocksDTO.number

    res.timestampUnix <- responseGetBlocksDTO.timestamp
    res.timestampNormal <-  DateTimeOffset.FromUnixTimeSeconds(responseGetBlocksDTO.timestamp.ToInt64()).UtcDateTime 

    res.baseFeePerGas <- responseGetBlocksDTO.baseFeePerGas
    res.gasLimit <- responseGetBlocksDTO.gasLimit
    res.gasUsed <- responseGetBlocksDTO.gasUsed

    res