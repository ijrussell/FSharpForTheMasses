(*
Feature: Eligible Registered customers must have an email address

Given the following Registered Customers
|Customer Id|Email        |Is Eligible|
|John       |john@test.org|true       |
|Mary       |mary@test.org|true       |
|Richard    |             |false      |
*)

type Email = string
type CustomerName = string

type EligibleRegisteredCustomer = {
    Name:CustomerName
    Email:Email
}

type RegisteredCustomer = {
    Name:CustomerName
    Email:Email option
}

type UnregisteredCustomer = {
    Name:string
}

type Customer =
    | Eligible of EligibleRegisteredCustomer
    | Registered of RegisteredCustomer
    | Guest of UnregisteredCustomer

let john = Eligible { Name = "John"; Email = "john@test.org" }
let mary = Eligible { Name = "Mary"; Email = "mary@test.org" }
let richard = Registered { Name = "Richard"; Email = None }
let sarah = Guest { Name = "Sarah" }

let calculateOrderTotal customer spend =
    let discount = 
        match customer with
        | Eligible _ when spend >= 100M -> spend * 0.1M
        | _ -> 0M
    spend - discount

let assertJohn = calculateOrderTotal john 100.0M = 90.0M
let assertMary = calculateOrderTotal mary 99.0M = 99.0M
let assertRichard = calculateOrderTotal richard 100.0M = 100.0M
let assertSarah = calculateOrderTotal sarah 100.0M = 100.0M
