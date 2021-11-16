namespace GiraffeView

[<RequireQualifiedAccess>]
module SharedViews =

    open Giraffe.ViewEngine

    // string -> XmlNode list -> XmlNode
    let createMasterPage msg content =
        html [] [
            head [] [
                title [] [ str msg ]
                link [ _rel "stylesheet"; _href "main.css" ]
            ]
            body [] content
        ]

