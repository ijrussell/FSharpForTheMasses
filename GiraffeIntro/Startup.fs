namespace GiraffeIntro

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open Giraffe.ViewEngine
open FSharp.Control.Tasks

module Views =

    let indexView =
        html [] [
            head [] [
                title [] [ str "Giraffe Example" ]
            ]
            body [] [
                h1 [] [ str "IS FUN |> F#" ]
                p [ _class "some-css-class"; _id "someId" ] [
                    str "Hello dotNET Oxford from the Giraffe View Engine"
                ]
            ]
        ]

module Handlers =

    let sayHelloNameHandler (name:string) : HttpHandler =
        fun (next:HttpFunc) (ctx:HttpContext) ->
            task {
                let msg = $"Hello {name}, how are you?"
                return! json {| Response = msg |} next ctx
            }
    
    let webApp = 
        choose [
            GET >=> choose [
                route "/" >=> htmlView Views.indexView
                route "/api" >=> json {| Response = "Hello dotNET Oxford!" |}
                routef "/api/%s" sayHelloNameHandler
            ]
            RequestErrors.NOT_FOUND "Not found"
        ]
    
type Startup() =

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    member _.ConfigureServices(services: IServiceCollection) =
        services.AddGiraffe() |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore
        app.UseGiraffe Handlers.webApp
