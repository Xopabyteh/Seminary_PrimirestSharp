## Authentication
### Admin login *(actions that require a login)*
When our application performs an action that requires someone to be logged in, such as 
loading their index page to fetch menu ids, we use `PrimirestAdminCredentailOptions` and
login with those credentials.

The application doesn't have actual admin privileges on their website, it uses Martin's credentials

The credentails have to be stored somewhere (like `secrets.json`) and loaded into the application
when it starts up.

#### secrets.json
```json
{
  "PrimirestAuthentication": {
    "AdminUsername": "some name",
    "AdminPassword": "some password"
  }
}
```

The actions that require a login should be ran with the `PerformAdminLoggedSession` method inside of `PrimirestAuthService`
