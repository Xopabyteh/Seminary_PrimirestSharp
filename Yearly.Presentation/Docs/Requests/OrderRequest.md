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
### Response
Ok or errors

## Cancel order
