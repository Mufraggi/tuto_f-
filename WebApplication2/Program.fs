module WebApplication2
open System
open Contact
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Text.Json.Serialization

open Giraffe
open Domain
open Microsoft.Extensions.Logging

let getListsForUserHandler (fetcher: ListFetcher) user =
    let lists = fetcher user
    let mapped = lists |> List.map Contact.fromDomain
    let response: GetListsResponse = { lists = mapped |> Array.ofSeq }
    json response

let createToDoListHandler (creator: Creator) : HttpHandler =
    fun next ctx ->
        task {
            let! todoList = ctx.BindJsonAsync<CreateToDoListRequest>()
            let createdTodoList = creator todoList
            return! json (fromDomain createdTodoList) next ctx
        }
    
let webApp (fetcher : ListFetcher)( creater :Creator)=
    choose [
        route "/ping"   >=> text "pong"
        GET >=> routef "/%s/lists" (getListsForUserHandler fetcher)
        POST >=>  route "/todos" >=>  createToDoListHandler creater
    ]
    
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text "Internal Server Error"

let configureApp fetcher creater (app : IApplicationBuilder) =
    app.UseGiraffeErrorHandler(errorHandler)
        .UseGiraffe(webApp fetcher creater)
                                                                   
 
let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter) // Optional filter
           .AddConsole()      // Set up the Console logger
           .AddDebug()        // Set up the Debug logger
    |> ignore
let jsonOptions =
    let options = Json.Serializer.DefaultOptions
    options.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.FSharpLuLike))
    options

let configureServices (services: IServiceCollection) =
    services
        .AddGiraffe()
        .AddSingleton<Json.ISerializer>(Json.Serializer(jsonOptions))
    |> ignore
let configure fetcher creater (webHostBuilder: IWebHostBuilder) =
    webHostBuilder
        .Configure(configureApp fetcher creater)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)

[<EntryPoint>]
let main _ =
    let fetcher = (fun _ -> [])
    let creator : Creator = fun todoList -> 
        { 
            name = todoList.Name
            description = todoList.Description
            status = Domain.Status.Doing
            percentageDone = 0m
        }
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(configure fetcher creator >> ignore)
        .Build()
        .Run()
    0