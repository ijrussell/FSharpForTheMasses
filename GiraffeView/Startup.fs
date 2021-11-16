namespace GiraffeView

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open FSharp.Control.Tasks
open TodoStore

module Handlers =

    let sayHelloNameHandler (name:string) : HttpHandler =
        fun (next:HttpFunc) (ctx:HttpContext) ->
            task {
                let msg = $"Hello {name}, how are you?"
                return! json {| Response = msg |} next ctx
            }
    
    let webApp = 
        choose [
            subRoute "/api/todo" Todos.apiTodoRoutes
            GET >=> choose [
                route "/" >=> htmlView (Todos.Views.todoView Todos.todoList)
                route "/api" >=> json {| Response = "Hello dotNET Oxford!" |}
                routef "/api/%s" sayHelloNameHandler
            ]
            RequestErrors.NOT_FOUND "Not found"
        ]
    
type Startup() =

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    member _.ConfigureServices(services: IServiceCollection) =
        services
            .AddGiraffe()
            .AddSingleton<TodoDataStore>(TodoDataStore()) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore
        app.UseStaticFiles() |> ignore
        app.UseGiraffe Handlers.webApp
