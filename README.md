<p align="center">
  <a href="https://github.com/Xopabyteh/Seminary_PrimirestSharp">
    <img src="https://raw.githubusercontent.com/Xopabyteh/Seminary_PrimirestSharp/refs/heads/master/Inkscape/AppIcons/logo_bubble_path.svg" />
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
But i am the only one working on the project... so i'm just using the code portions of DDD.

### Features
 - [x] Ordering through primirest 🥓
 - [x] User published photos 📷
 - [x] Notifications when you forgot to order 📩
 - [x] Food aliasing for photo reusability ♻
 - [x] Slick client app 🔥
 - [x] Dark mode 🌝 

## Disclaimer 🫂
This application is a student project and is not affiliated with Primirest, Delirest, or any other companies in their network.

Systems designed in this project operate entirely separately from the official Primirest 
system and do not integrate with or alter the original platform's backend processes.

This application is provided free of charge and is intended solely for educational and personal use. It is non-commercial, meaning it generates no profit and is not sold, distributed, or promoted via any commercial channels, including app stores or other retail platforms. Users should be aware that, as a personal project, the app may contain bugs or incomplete features, and support is limited to the developer's availability and capacity.

By using this application, you acknowledge and accept that it is provided "as-is" without any warranties or guarantees of any kind. The developer disclaims all liability for any damages or losses that may arise from the use of this app. Users are encouraged to report any issues or provide feedback to help improve the application, but there are no obligations or commitments from the developer to implement such changes or provide ongoing support.

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
  "Persistence:AzureStorageConnectionString:blob": "Connection string",
  "Persistence:DbConnectionString": "MS SQL Server Connection string",

  //These refer to a Primirest account used to store foods from Primirest to P#
  "PrimirestAuthentication:AdminPassword": "Password to your primirest account",
  "PrimirestAuthentication:AdminUsername": "Name of your primirest account"

  "NotificationHub:FullAccessConnectionString": "Connection string"
  "ConnectionStrings:ApplicationInsights": "Connection string"
```

##### Azure storage
In the Azure storage account there must be a **Blob container** called **food-photos**. 
The app will create the container if not present on startup.

For local development i am locally hosting an Azure storage image inside of docker.

#### Cmd
```bash
$ dotnet run --project ~PathToPresentationProject~\Yearly.Presentation.csproj --launch-profile https
```
