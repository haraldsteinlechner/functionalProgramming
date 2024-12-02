module Main exposing (main)

import Dict
import Browser
import Html exposing (..)
import Html.Events exposing (..)
import Html.Attributes exposing (..)
import Json.Decode

type alias Todo =
    {
        name : String,
        checked : Bool
    }

type alias Model =
    { 
        currentName : String,
        currentId : Int,
        todos : Dict.Dict Int Todo
    }


initialModel : Model
initialModel =
    { 
        currentName = "",
        currentId = 0,
        todos = Dict.empty
    }

type Msg
    = SetName String
    | AddTodo
    | CheckTodo Int
    | DeleteTodo Int
    | Nop

update : Msg -> Model -> Model
update msg model =
    case msg of
        Nop -> model
        SetName name -> 
            { model | currentName = name }
        AddTodo -> 
            if model.currentName == "" then model
            else 
                let 
                    newTodo = 
                        { 
                            name = model.currentName,  
                            checked = False
                        }
                in
                    {
                        model |
                            todos = 
                                Dict.insert 
                                    model.currentId
                                    newTodo
                                    model.todos,
                            currentName = "",
                            currentId = model.currentId + 1
                    }
        DeleteTodo id -> 
            { model | 
                todos = Dict.remove id model.todos 
            }
        CheckTodo id -> 
            case Dict.get id model.todos of
                Nothing -> model
                Just old -> 
                    let 
                        new = { old | checked = not old.checked }
                    in
                        { model | todos = Dict.insert id new model.todos } 
        

countOpenTodos model =
    model.todos
        |> Dict.values
        |> List.filter (\ todo -> not todo.checked)
        |> List.length

view : Model -> Html Msg
view model =
    div []
        [
            text "Todo:",
            input [
                onInput SetName,
                value model.currentName,
                on "keydown" (Json.Decode.map (\ c -> if c == 13 then AddTodo else Nop) keyCode)
            ] [],
            div [] (model.todos |> Dict.toList |> List.map (\ (id,todo) -> 
                div [] [
                    button [onClick (CheckTodo id)] [text "v"],
                    button [onClick (DeleteTodo id)] [text "X"],
                    div (
                        if todo.checked then
                            [style "text-decoration" "line-through"]
                        else
                            []
                    )
                        [ text todo.name]
                ]
            )),
            text (model |> countOpenTodos |> String.fromInt)
        ]


main : Program () Model Msg
main =
    Browser.sandbox
        { init = initialModel
        , view = view
        , update = update
        }
