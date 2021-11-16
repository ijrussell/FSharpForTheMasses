type Customer = {
    Id : int
    IsVip : bool
    Credit : decimal
}

// Build a pipline of functions
// getPurchases >> tryPromoteToVip >> increaseCreditIfVip

// Customer -> (Customer * decimal)
let getPurchases customer =
    let puchases = if customer.Id % 2 = 0 then 120M else 80M
    (customer, puchases)

// (Customer * decimal) -> Customer
let tryPromoteToVip customerPurchases =
    let (customer, amount) = customerPurchases
    if amount > 100M then { customer with IsVip = true }
    else customer

// Customer -> Customer
let increaseCreditIfVip customer =
    let increase = if customer.IsVip then 100M else 50M
    { customer with Credit = customer.Credit + increase }

let composeV1 customer =
    let result1 = getPurchases customer
    let result2 = tryPromoteToVip result1
    let result3 = increaseCreditIfVip result2
    result3

let composeV2 customer =
    increaseCreditIfVip(tryPromoteToVip(getPurchases(customer)))

let composeV3 customer =
    customer
    |> getPurchases 
    |> tryPromoteToVip 
    |> increaseCreditIfVip 

let composeV4 =
    getPurchases >> tryPromoteToVip >> increaseCreditIfVip

let vip = { Id = 1; IsVip = true; Credit = 200M }

let assertVip1 = (composeV1 vip = { vip with Credit = 300M })
let assertVip2 = (composeV2 vip = { vip with Credit = 300M })
let assertVip3 = (composeV3 vip = { vip with Credit = 300M })
let assertVip4 = (composeV4 vip = { vip with Credit = 300M })

let promote = { Id = 2; IsVip = false; Credit = 120M } 

let assertPromote1 = (composeV1 promote = { promote with IsVip = true; Credit = 220M })
let assertPromote2 = (composeV2 promote = { promote with IsVip = true; Credit = 220M })
let assertPromote3 = (composeV3 promote = { promote with IsVip = true; Credit = 220M })
let assertPromote4 = (composeV4 promote = { promote with IsVip = true; Credit = 220M })

let notPromote = { Id = 3; IsVip = false; Credit = 75M }

let assertNotPromote1 = (composeV1 notPromote = { notPromote with Credit = 125M })
let assertNotPromote2 = (composeV2 notPromote = { notPromote with Credit = 125M })
let assertNotPromote3 = (composeV3 notPromote = { notPromote with Credit = 125M })
let assertNotPromote4 = (composeV4 notPromote = { notPromote with Credit = 125M })



