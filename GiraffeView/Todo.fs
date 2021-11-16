namespace GiraffeView

open System
open System.Collections.Generic
open Microsoft.AspNetCore.Http
open Giraffe
open FSharp.Control.Tasks
open TodoStore

module Todos =

    module Views =

        open Giraffe.ViewEngine

        // Todo -> XmlNode
        let showListItem (todo:Todo) =
            let style = if todo.IsCompleted then [ _class "checked" ] else []
            li style [ str todo.Description ]

        let todoView items =
            [
                div [ _id "myDIV"; _class "header" ] [
                    h2 [] [ str "My To Do List" ]
                    input [ _type "text"; _id "myInput"; _placeholder "Title..." ]
                    span [ _class "addBtn"; _onclick "newElement()" ] [ str "Add" ]
                ]
                ul [ _id "myUL" ] [
                    for todo in items do
                        showListItem todo
                ]
                script [ _src "main.js"; _type "text/javascript" ] []
            ] 
            |> SharedViews.createMasterPage "My ToDo App"

    let todoList =
        let create description isCompleted = {
                Id = Guid.NewGuid()
                Description = description
                Created = DateTime.UtcNow
                IsCompleted = isCompleted
            }
        [
            create "Hit the gym" false
            create "Pay bills" true
            create "Meet George" false
            create "Buy eggs" false
            create "Read a book" true
            create "Read Essential Functional-First F#" false
        ]

    let viewTodosHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
            let store = ctx.GetService<TodoDataStore>()
            let todos = store.GetAll()
            return! json todos next ctx
        }
    
    let viewTodoHandler (id:Guid) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let store = ctx.GetService<TodoDataStore>()
                let todo = store.Get(id)
                return! json todo next ctx
            }
    
    let createTodoHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! newTodo = ctx.BindJsonAsync<NewTodo>()
                let store = ctx.GetService<TodoDataStore>()
                let created =
                    store.Create({
                        Id = Guid.NewGuid()
                        Description = newTodo.Description
                        Created = DateTime.UtcNow
                        IsCompleted = false })
                return! json created next ctx
            }
    
    let updateTodoHandler (id:Guid) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! todo = ctx.BindJsonAsync<Todo>()
                let store = ctx.GetService<TodoDataStore>()
                let created = store.Update(todo)
                return! json created next ctx
            }
    
    let deleteTodoHandler (id:Guid) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let store = ctx.GetService<TodoDataStore>()
                let existing = store.Get(id)
                let deleted = store.Delete(KeyValuePair<TodoId, Todo>(id, existing))
                return! json deleted next ctx
            }

    let apiTodoRoutes : HttpHandler =
        choose [
            GET >=> choose [
                routef "/%O" viewTodoHandler
                route "" >=> viewTodosHandler
            ]
            POST >=> route "" >=> createTodoHandler
            PUT >=> routef "/%O" updateTodoHandler
            DELETE >=> routef "/%O" deleteTodoHandler
        ]
