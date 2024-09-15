module Contract

open Domain

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

type GetListResponse = {lists: ToDoList []}

let fromDomain (list: Domain.ToDoList ) =
      let mapStatus (s: Domain.Status) =
          match s with
          | Domain.Todo -> Todo
          | Domain.Doing -> Doing
          | Domain.Done -> Done
          
      { name= list.name
        description= list.description
        status= list.status|>mapStatus
        percentageDone= list.percentageDone
}

type GetListsResponse = { lists: ToDoList [] }