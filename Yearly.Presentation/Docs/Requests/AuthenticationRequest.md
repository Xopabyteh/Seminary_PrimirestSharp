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
  "username": "Martin Fiala",
  "sessionCookie": "ASP.NET_SessionId=x..x"
}
```

# Logout