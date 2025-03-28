namespace CryptChat.Services

open System
open System.Diagnostics
open System.Net.Http
open System.Text.Json

module NgrokService =
    let private ngrokExecutablePath = @"C:\ProgramData\chocolatey\bin\ngrok.exe"
    let private ngrokApiUrl = "http://localhost:4040/api/tunnels"

    let getNgrokUrlAsync () =
        async {
            let processStartInfo = ProcessStartInfo(FileName = ngrokExecutablePath, Arguments = "http https://localhost:5000")
            use process = new Process(StartInfo = processStartInfo)
            process.Start() |> ignore

            do! Async.Sleep 5000

            use client = new HttpClient()
            let! ngrokStatusResponse = client.GetStringAsync(ngrokApiUrl) |> Async.AwaitTask

            use doc = JsonDocument.Parse(ngrokStatusResponse)
            let publicUrl = 
                (doc.RootElement.GetProperty("tunnels")).[0]

            let result = publicUrl.GetProperty("public_url").GetString()

            return result
        }