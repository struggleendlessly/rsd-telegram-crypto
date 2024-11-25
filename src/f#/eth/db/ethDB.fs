module ethDB

open Microsoft.EntityFrameworkCore

type MyEntity() =
    member val Id: int = 0 with get, set
    member val Name: string = null with get, set

type ethDB(options: DbContextOptions<ethDB>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable myEntities: DbSet<MyEntity>
    member this.MyEntities with get() = this.myEntities and set v = this.myEntities <- v
