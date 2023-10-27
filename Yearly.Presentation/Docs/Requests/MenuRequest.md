## Get Menu

```json
GET {{host}}/menu?sessionCookie={{sessionCookie}}
```


### Response
```json
{
    "menusForWeeks": [
    {
        "menusForDays": [
            {
                "date": "2023-10-30T23:00:00",
                "foods": [
                    {
                        "name": "Tagliatelle s kuøecím masem a rajèaty",
                        "allergens": "1a,3,7",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104168,
                            "itemId": 119104174
                        }
                    },
                    {
                        "name": "Vepøová kýta dušená s krémovou omáèkou se zeleninou (Bratislavská), dušená rýže",
                        "allergens": "1a,7,9,10",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104168,
                            "itemId": 119104173
                        }
                    },
                    {
                        "name": "Restovaná zelenina s èervenou fazolí a jogurtovým dipem, vaøené brambory",
                        "allergens": "1a,7",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104168,
                            "itemId": 119104175
                        }
                    }
                ]
            },
            {
                "date": "2023-10-31T23:00:00",
                "foods": [
                    {
                        "name": "Losos peèený na másle, šouchané brambory s cibulkou, pøízdoba",
                        "allergens": "1a,4,7,9",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104176,
                            "itemId": 119104181
                        }
                    },
                    {
                        "name": "Vepøové výpeèky, dušené èervené zelí, domácí bramborové knedlíky",
                        "allergens": "1a,3,9",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104176,
                            "itemId": 119104182
                        }
                    },
                    {
                        "name": "Lasagne se špenátem a rajèaty",
                        "allergens": "1a,3,7,9",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104176,
                            "itemId": 119104183
                        }
                    }
                ]
            },
            {
                "date": "2023-11-01T23:00:00",
                "foods": [
                    {
                        "name": "Èínské nudle s kuøecím masem a zeleninou",
                        "allergens": "1a,6",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104184,
                            "itemId": 119104189
                        }
                    },
                    {
                        "name": "Dýòové rizoto se sýrem, pøízdoba",
                        "allergens": "1a,7",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104184,
                            "itemId": 119104191
                        }
                    },
                    {
                        "name": "Králièí stehna peèená na èesneku, šouchané brambory s pažitkou",
                        "allergens": "1a,7",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104184,
                            "itemId": 119104190
                        }
                    }
                ]
            },
            {
                "date": "2023-11-02T23:00:00",
                "foods": [
                    {
                        "name": "Vepøové maso na houbách, domácí houskové knedlíky",
                        "allergens": "1a,1d,3,7,9,12",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104192,
                            "itemId": 119104198
                        }
                    },
                    {
                        "name": "Kvìtákovo-brokolicový nákyp s karamelizovanou cibulkou, pøízdoba (zapeèená smìs brokolice,  kvìtáku a brambor)",
                        "allergens": "1a,1d,3,7,9,12",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104192,
                            "itemId": 119104197
                        }
                    },
                    {
                        "name": "Gratinované špecle s kuøecí smìsí se zeleninou",
                        "allergens": "1a,1d,3,6,7,9,10,12",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104192,
                            "itemId": 119104199
                        }
                    }
                ]
            },
            {
                "date": "2023-11-03T23:00:00",
                "foods": [
                    {
                        "name": "Fazolové lusky na kyselo, vaøené vejce, vaøené brambory",
                        "allergens": "1a,3,7,9",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104200,
                            "itemId": 119104207
                        }
                    },
                    {
                        "name": "Maïarský hovìzí guláš, vaøené tìstoviny",
                        "allergens": "1a,9",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104200,
                            "itemId": 119104205
                        }
                    },
                    {
                        "name": "Kuøecí palièky v gyros koøení a jogurtu, dušená rýže",
                        "allergens": "1a,7,9",
                        "photoLinks": [],
                        "primirestFoodIdentifier": {
                            "menuId": 119104167,
                            "dayId": 119104200,
                            "itemId": 119104206
                        }
                    }
                ]
            }
        ],
        "primirestMenuId": 119104167
    },
}
```
> Returns all the menus, that P# is aware of.
So the menus for this week and possibly for another. 
If there is no menu, an empty array.

> Returns `menus for weeks`, each `menu for week` contains a list of `menus for day`
Each `menu for day` contains a list of `foods` and `date` of the day.

> The `primirestFoodIdentifier` contains all necessary data to order the food.
