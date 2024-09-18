module Contact 
type Status = string // We'll use string representation in the API

type ToDoListDto = {
    name: string
    description: string
    status: Status
    percentageDone: decimal
}

type CreateToDoListRequest = {
    name: string
    description: string
}

type GetListsResponse = {
    lists: ToDoListDto array
}

let fromDomain (todoList: Domain.ToDoList) : ToDoListDto =
    { 
        name = todoList.name
        description = todoList.description
        status = 
            match todoList.status with
            | Domain.Status.Doing -> "Doing"
            | Domain.Status.Done -> "Done"
            | Domain.Status.Todo -> "Todo"
        percentageDone = todoList.percentageDone
    }

let toDomain (request: CreateToDoListRequest) : Domain.ToDoList =
    { 
        name = request.name
        description = request.description
        status = Domain.Status.Todo
        percentageDone = 0m
    }