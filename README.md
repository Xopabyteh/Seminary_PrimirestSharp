
<p align="center">
  <a href="https://github.com/Xopabyteh/Seminary_PrimirestSharp">
    <img src="https://cdn.discordapp.com/attachments/826838935572840492/1201956776795459625/logo_bubble.svg?ex=65cbb47a&is=65b93f7a&hm=4670a67fe98526d2f10343e3afc9abfd52c32009f596a9516f435ab7c5e357d9&" />
  </a>
</p>

<h3 align="center">Finally food ordering with images</h3>
<hr>

## About
The project acts as a wrapper application around the [Primirest](https://www.mujprimirest.cz/) system + with additional features.
The main bonus is the ability of users of this application to take photos of their ordered foods
so that everyone else ordering it can see it.

The project is split into two parts - a custom backend wrapper api and a frontend Maui app.

This is my Seminary work for mensa gymnázium `Sexta 2023 - Septima 2025`.
The project is built with clean architecture and some DDD concepts in mind.
But i am the only one working on the project and i am my own domain expert,
so i'm just using portions of DDD.

## Docs
Most of the docs are made with figma available at the links below. Some docs can be found in project folders specific to that layer of the application.

* [Design files](https://www.figma.com/file/K7Y98Sp4qY1c6XDhdkm9wV/Unleashed-Dine-Maui?type=design&node-id=0-1&mode=design&t=rVnyGwpnEonobXKa-0)
> UI and app design

* [Project architecture](https://www.figma.com/file/FuD7lmST0Ar9oFFZS6Jlt3/Unleashed-Diner-Flow?type=whiteboard&node-id=907-53&t=iEvTKeGkVjXp1MAQ-0)
> Project architecture, domain models, implementations

* [Process modeling](https://www.figma.com/file/iXr6mEJRbgFyzvCq5Mynn4/Primirest-sharp-Flow?type=design&node-id=39-52344&mode=design&t=98IKJbIVTUfWHq9b-0)
> Process modeling, domain events storming

### Running the Backend
#### Dependencies & User secrets
Parameters that are used by the app are all defined in the appsettings.json where it also states which portions are set using user secrets (or Azure keyvault or whatever)

User secrets should look like this
```json
  "Persistence:AzureStorageConnectionString": "Connection string",
  "Persistence:AzureStorageConnectionString:blob": "Connection string",
  "Persistence:DbConnectionString": "MS SQL Server Connection string",
  // "Persistence:RedisConnectionString": "Connection string", //Note, curently not in use

  //These refer to a Primirest account used to store foods from Primirest to P#
  "PrimirestAuthentication:AdminPassword": "Password to your primirest account",
  "PrimirestAuthentication:AdminUsername": "Name of your primirest account"
```

For local development i am locally hosting Azure storage and Redis images inside of docker.

#### Cmd
Without seeding:
```bash
$ dotnet run --project ~PathToPresentationProject~\Yearly.Presentation.csproj --launch-profile https
```
With admin user seeding:
```bash
$ dotnet run --project ~PathToPresentationProject~\Yearly.Presentation.csproj --launch-profile https seedProfile=adminuser
```
##### Arguments
* seedProfile - the profile to seed the database with, if not provided, the database will not be seeded
	* There are various profiles, the profiles and the way they work can be found in the `Yearly.Infrastructure` project in `Persistence\Seeding\DataSeeder.cs`
