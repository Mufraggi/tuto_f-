module WebApplication2.Tests

open Domain
open Contract
open System.Text.Json
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost

open Xunit
open Swensen.Unquote
[<Fact>]    
let  ``Can get all list of user`` () =
    let lists: Domain.ToDoList list =
        [ { name = "books"
            description = "my bookshelf"
            status = Domain.Status.Todo
            percentageDone = 0m } ]

    let db = [ ("jo", lists) ] |> Map.ofList

    let mapFetcher username = db |> Map.find username
    let whb = new WebHostBuilder() |> configure mapFetcher 
    let server = new TestServer(whb)
    let client = server.CreateClient()
    
    let response =
        task {
            let! response = client.GetAsync(@"/jo/lists")
            test <@ response.StatusCode = System.Net.HttpStatusCode.OK @>
            let! stream = response.Content.ReadAsStreamAsync()
            let! content = JsonSerializer.DeserializeAsync<GetListResponse>(stream, jsonOptions)
            return content
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously
   
    test <@ response= {lists =   [|{
             name = "books"
             description = "my bookshelf"
             status = Status.Todo
             percentageDone = 0m
         }|] } @>