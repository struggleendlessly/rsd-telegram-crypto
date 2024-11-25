module IScopedProcessingService

open System.Threading
open System.Threading.Tasks

type IScopedProcessingService =
    abstract member DoWorkAsync: CancellationToken -> Task
