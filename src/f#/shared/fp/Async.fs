module Async

    let map f op = async {
        try
            let! x = op
            let value = f x
            return value
        with
        | ex ->

            return raise ex
    }


    let (>=>) f g x = async { 
        let! y = f x 
        return! g y 
    }