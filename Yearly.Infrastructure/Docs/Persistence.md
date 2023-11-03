## Persistence

#### secrets.json 
This section must be present in the secrets.json file (or appsettings.json) to establish a connection to the database
```json
  "Persistence": {
    "DbConnectionString": "Server=LYNX-GRUNEX;Database=PrimirestSharp;Trusted_Connection=True;TrustServerCertificate=True"
  }
```