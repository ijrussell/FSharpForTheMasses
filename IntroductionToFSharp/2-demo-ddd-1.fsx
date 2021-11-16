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

// Record type (AND)
type Customer = {
    Name:string
    Email:string option
    IsEligible:bool
    IsRegistered:bool
}

let calculateOrderTotal customer spend =
    let discount = 
        if customer.IsRegistered && customer.IsEligible && spend >= 100M then spend * 0.1M
        else 0M
    spend - discount

let john = { Name = "John"; Email = Some "john@test.org"; IsEligible = true; IsRegistered = true }
let mary = { john with Name = "Mary"; Email = Some "mary@test.org" }
let richard = { john with Name = "Richard"; Email = None; IsEligible = false }
let sarah = { richard with Name = "Sarah"; IsRegistered = false }

let assertJohn = calculateOrderTotal john 100.0M = 90.0M
let assertMary = calculateOrderTotal mary 99.0M = 99.0M
let assertRichard = calculateOrderTotal richard 100.0M = 100.0M
let assertSarah = calculateOrderTotal sarah 100.0M = 100.0M

// type Option<'T> =
//     | Some of 'T
//     | None
