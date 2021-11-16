module GiraffeView.TodoStore

open System
open System.Collections.Concurrent

type TodoId = Guid

type NewTodo = {
    Description:string
}

type Todo = {
    Id:TodoId
    Description:string
    Created:DateTime
    IsCompleted:bool
}

type TodoDataStore() =
    let data = ConcurrentDictionary<TodoId, Todo>()
    
    member _.Create todo = data.TryAdd(todo.Id, todo)
    member _.Update todo = data.TryUpdate(todo.Id, todo, data.[todo.Id])
    member _.Delete id = data.TryRemove id
    member _.Get id = data.[id]
    member _.GetAll () = data.ToArray()
