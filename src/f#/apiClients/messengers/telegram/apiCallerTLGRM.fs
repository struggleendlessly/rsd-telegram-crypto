module apiCallerTLGRM

open System
open System.Net.Http
open Microsoft.Extensions.Logging
open System.Text

//let urlBuilder (threadId: string) (chat_id: string)  (text: string) apiKey = 
//    sprintf """bot%s/sendMessage?
//            message_thread_id=%s&
//            chat_id=%s&
//            text=%s&
//            parse_mode=MarkDown&
//            disable_web_page_preview=true"""
//            apiKey
//            threadId 
//            chat_id 
//            text

let urlBuilder (threadId: string) (chat_id: string)  (text: string) (apiKey: string)=
    let sb = StringBuilder()
    sb.Append("bot") |> ignore
    sb.Append(apiKey) |> ignore
    sb.Append("/sendMessage?") |> ignore
    sb.Append("message_thread_id=") |> ignore
    sb.Append(threadId) |> ignore
    sb.Append("&chat_id=") |> ignore
    sb.Append(chat_id) |> ignore
    sb.Append("&text=") |> ignore
    sb.Append(text) |> ignore
    sb.Append("&parse_mode=MarkDown&") |> ignore
    sb.Append("disable_web_page_preview=true") |> ignore
    sb.ToString()

let request 
    (logger: ILogger)
    (apiKeys: string[])
    url
    (httpClientFactory: IHttpClientFactory)
    (queryParams: string -> string) = 
  
        task {
            apiKeys 
                |> Random().Shuffle

            use client = httpClientFactory.CreateClient "Api"
            client.BaseAddress <- url |> Uri
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8") |> ignore

            let requestStr = queryParams (Array.item 0 apiKeys)

            logger.LogInformation( "Request: {s}", requestStr)

            let! response = client.GetAsync(requestStr) 
            let! content = response.Content.ReadAsStringAsync() 

            //logger.LogInformation( "response: {s}", content)

            return content
        } 
        |> Async.AwaitTask

let icons = [|
    ("greenBook", "%F0%9F%93%97")
    ("redBook", "%F0%9F%93%95")
    ("lightning", "%E2%9A%A1")
    ("coin", "%F0%9F%AA%99")
    ("chart", "%F0%9F%92%B9")
    ("whiteCircle", "%E2%9A%AA")
    ("yellowCircle", "%F0%9F%9F%A1")
    ("orangeCircle", "%F0%9F%9F%A0")
    ("redCircle", "%F0%9F%94%B4")
    ("star", "%E2%9C%A8")
    ("snowflake", "%E2%9C%B3")
    ("poops", "%F0%9F%92%A9")
    ("rocket", "%F0%9F%9A%80")
    ("flagRed", "%F0%9F%9A%A9")
    ("buy", "%F0%9F%93%88")
    ("sell", "%F0%9F%93%89")
    ("squareYellow", "%F0%9F%9F%A8")
    ("squareOrange", "%F0%9F%9F%A7")
    ("squareRed", "%F0%9F%9F%A5")
    ("fire", "%F0%9F%94%A5")
    ("boom", "%F0%9F%92%A5")
    ("clockSend", "%E2%8F%B3")
    ("calendar", "%F0%9F%97%93")
    ("antenna", "%F0%9F%93%B6")
    ("SCROLL", "%F0%9F%93%9C")
|]

