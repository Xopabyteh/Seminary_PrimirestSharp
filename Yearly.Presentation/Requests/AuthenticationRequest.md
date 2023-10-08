# Login

## Login with username and password
```json
POST {{host}}/auth/login
Content-Type: application/json

{
    "username": "Martin Fiala",
    "password": "User password"
}

```
### Response
```json
{
  "id": "0",
  "username": "Martin Fiala",
  "sessionCookie": "ASP.NET_SessionId=x..x"
}
```