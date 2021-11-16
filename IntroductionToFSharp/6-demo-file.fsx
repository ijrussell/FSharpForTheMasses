open System
open System.IO

type Customer = {
    Name:string
    Email:string option
    IsEligible:bool
    IsRegistered:bool
    DateRegistered:DateTime option
    Discount:decimal option
}

type DataError =
    | ParsingError of string

// string -> seq<string>
let readFile path = 
    seq {
        use reader = new StreamReader(File.OpenRead(path))
        while not reader.EndOfStream do
            reader.ReadLine()
    }

let parse (input:string) =
    match input.Split("|") with
    | [| name;email;isEligible;isRegistered;dateRegistered;discount |] ->
        Ok {
            Name = name
            Email = if email <> "" then Some email else None
            IsEligible = isEligible = "1"
            IsRegistered = isRegistered = "1"
            DateRegistered = 
                let success, value = DateTime.TryParse dateRegistered
                if success then Some value else None
            Discount = 
                let success, value = Decimal.TryParse discount
                if success then Some value else None
        }
    | data -> Error (ParsingError $"'%A{data}' cannot be parsed")

let processData () =
    Path.Combine(__SOURCE_DIRECTORY__, "resources", "data.csv")
    |> readFile
    |> Seq.skip 1
    |> Seq.map parse
    |> Seq.iter (printfn "%A")

processData ()

