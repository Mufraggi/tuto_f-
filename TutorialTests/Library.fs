module WebApplication2.Tests

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Xunit
open Swensen.Unquote
[<Fact>]
let hello_word_tests() =
    let whb = new WebHostBuilder() |> configure 
    let server = new TestServer(whb)
    let client = server.CreateClient()
    
    let response =
        task {
            let! response = client.GetAsync(@"/ping")
            let! content = response.Content.ReadAsStringAsync()
            return content
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously
   
    test <@ response= "pong" @>