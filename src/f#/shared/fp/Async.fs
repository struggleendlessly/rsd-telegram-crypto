module Async

    let map f op = async {
        let! x = op
        let value = f x
        return value
    }

    let (>=>) f g x = async { 
        let! y = f x 
        return! g y 
    }