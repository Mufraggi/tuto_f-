module Domain


type Status =
    | Todo
    | Doing
    | Done
    
type ToDoList = {
    name: string
    description: string
    status: Status
    percentageDone:decimal
}
type ListFetcher = string -> ToDoList list
type CreateToDoListRequest = { Name: string; Description: string }

type Creator = CreateToDoListRequest -> ToDoList


