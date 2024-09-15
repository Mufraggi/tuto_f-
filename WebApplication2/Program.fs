module WebApplication2
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Text.Json.Serialization

open Giraffe
open Domain

let getListsForUserHandler (fetcher: ListFetcher) user =
    let lists = fetcher user
    let mapped = lists |> List.map Contract.fromDomain
    let response: Contract.GetListsResponse = { lists = mapped |> Array.ofSeq }
    json response
    
let webApp (fetcher : ListFetcher) =
    choose [
        route "/ping"   >=> text "pong"
        routef "/%s/lists" (getListsForUserHandler fetcher)
    ]
    

let configureApp fetcher (app : IApplicationBuilder) = app.UseGiraffe(webApp fetcher) 

let jsonOptions =
    let options = Json.Serializer.DefaultOptions
    options.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.FSharpLuLike))
    options

let configureServices (services: IServiceCollection) =
    services
        .AddGiraffe()
        .AddSingleton<Json.ISerializer>(Json.Serializer(jsonOptions))
    |> ignore
let configure fetcher (webHostBuilder: IWebHostBuilder) =
    webHostBuilder
        .Configure(configureApp fetcher)
        .ConfigureServices(configureServices)

[<EntryPoint>]
let main _ =
    let fetcher = (fun _ -> [])
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fetcher >> ignore)
        .Build()
        .Run()
    0