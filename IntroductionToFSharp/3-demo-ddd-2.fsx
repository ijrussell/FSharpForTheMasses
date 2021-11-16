(*
Feature: Applying a discount

Scenario: Eligible Registered Customers get 10% discount 
when they spend £100 or more

Given the following Registered Customers
|Customer Id|Email        |Is Eligible|
|John       |john@test.org|true       |
|Mary       |mary@test.org|true       |
|Richard    |             |false      |

When <Customer Id> spends <Spend>
Then their order total will be <Total>

Examples:
|Customer Id| Spend | Total |
|Mary       |  99.00|  99.00|
|John       | 100.00|  90.00|
|Richard    | 100.00| 100.00|
|Sarah      | 100.00| 100.00|
*)

type RegisteredCustomer = {
    Name:string
    Email:string option
    IsEligible:bool
}

type UnregisteredCustomer = {
    Name:string
}

type Customer =
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer

let calculateOrderTotal customer spend =
    let discount = 
        match customer with
        | Registered c when c.IsEligible && spend >= 100M -> spend * 0.1M
        | Registered _ -> 0M
        | Guest _ -> 0M
    spend - discount

let john = Registered { Name = "John"; Email = Some "john@test.org"; IsEligible = true }
let mary = Registered { Name = "Mary"; Email = Some "mary@test.org"; IsEligible = true }
let richard = Registered { Name = "Richard"; Email = None; IsEligible = false }
let sarah = Guest { Name = "Sarah" }

let assertJohn = calculateOrderTotal john 100.0M = 90.0M
let assertMary = calculateOrderTotal mary 99.0M = 99.0M
let assertRichard = calculateOrderTotal richard 100.0M = 100.0M
let assertSarah = calculateOrderTotal sarah 100.0M = 100.0M

// type Option<'T> =
//     | Some of 'T
//     | None

// // Customer -> unit
// let showEmail customer =
//     match customer with
//     | Registered c when c.Email.IsSome -> printfn "%s" $"Sending email to {c.Email.Value}" 
//     | _ -> ()

// let showEmail customer =
//     match customer with
//     | Registered c -> 
//         match c.Email with
//         | Some email -> printfn "%s" $"Sending email to {email}"
//         | None -> ()
//     | Guest _ -> ()

// let showEmail customer =
//     match customer with
//     | Registered c -> 
//         c.Email |> Option.iter (fun email -> printfn "%s" $"Sending email to {email}") 
//     | Guest _ -> ()

// let resultSome = showEmail mary
// let resultNone = showEmail richard

