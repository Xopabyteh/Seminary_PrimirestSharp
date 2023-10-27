## Get ordered foods (my orders)
```json
GET {{host}}/order/my-orders?sessionCookie={{sessionCookie}}&menuForWeekId=1234
```
### Response
```json
{
    "orders": [
        {
            "orderItemId": 121445269,
            "orderId": 121445268,
            "foodItemId": 119104181
        },
        {
            "orderItemId": 121445271,
            "orderId": 121445270,
            "foodItemId": 119104189
        }
    ]
}
```

> Returns all necessary data to show ordered foods (`foodItemId`) and 
to cancel an order (`orderItemId` and `orderId`, 
[the `menuId` is part of the request, so you already have it])

> `orderItemId` and `orderId` are used when cancelling an order
`foodItemId` is the food, that is actually ordered.

## Order food
```json
POST {{host}}/order/new-order
{
	"sessionCookie": "ASP.NET_SessionId=...",
	"primirestOrderIdentifier": {
		"menuId": 119104167,
        "dayId": 119104200,
        "itemId": 119104207
	}
}
```
> Returns Ok or errors

## Cancel order
