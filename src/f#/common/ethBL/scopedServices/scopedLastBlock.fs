module scopedLastBlock

open System
open System.Linq
open System.Threading

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open Microsoft.EntityFrameworkCore

open alchemy
open ethCommonDB
open ethCommonDB.models

open IScopedProcessingService
open bl_createSeq
open Extensions
open responseGetBlock
open mapResponseGetBlock
open ChainSettingsOptionModule

type BlockDetectionResult = 
    | NewBlocks of responseGetBlocks[] 
    | NoNewBlocks

type scopedLastBlock(
        logger: ILogger<scopedLastBlock>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastKnownBlockInDB  =
        let noBlock = BlocksEntity.Default(int chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: BlocksEntity) = x.numberInt

        ethDB.blocksEntities
                .OrderByDescending(fun x -> x.numberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastEthBlock = 
        async {
                let! a = alchemy.getLastBlockNumber()                      
                return a.blockInt
        }

    let saveToDB blocks = 
        async {
            if Array.isEmpty blocks then
                return 0
            else
                do! ethDB.blocksEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }

    let getBlocks n startBlock = 
         getSeqToProcess1 n startBlock
         >> Async.Bind alchemy.getBlockByNumber  
         >> Async.map mapBlocks
         >> Async.Bind saveToDB

    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let res = getBlocks 1000
                                getLastKnownBlockInDB
                                getLastEthBlock
                          |> Async.RunSynchronously

                return res
            }
