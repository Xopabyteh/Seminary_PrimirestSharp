## Get Menu

```json
GET {{host}}/menu?sessionCookie={{sessionCookie}}
```


### Response
```json
{
    "menus": [
        {
            "date": "2023-11-10T23:00:00",
            "foods": [
                {
                    "name": "Krupoto s peèenou èervenou øepou a sýrem typu Feta",
                    "allergens": "1c,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119137656,
                        "dayId": 119137727,
                        "itemId": 119137738
                    }
                },
                {
                    "name": "Zapeèené tìstoviny s mletým masem a pórkem",
                    "allergens": "1a,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119137656,
                        "dayId": 119137727,
                        "itemId": 119137737
                    }
                },
                {
                    "name": "Filet Mahi Mahi s bylinkovým máslem, vaøené brambory s petrželkou",
                    "allergens": "4,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119137656,
                        "dayId": 119137727,
                        "itemId": 119137736
                    }
                }
            ]
        },
        {
            "date": "2023-10-30T23:00:00",
            "foods": [
                {
                    "name": "Restovaná zelenina s èervenou fazolí a jogurtovým dipem, vaøené brambory",
                    "allergens": "1a,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119104167,
                        "dayId": 119104168,
                        "itemId": 119104175
                    }
                },
                {
                    "name": "Vepøová kýta dušená s krémovou omáèkou se zeleninou (Bratislavská), dušená rýže",
                    "allergens": "1a,7,9,10",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119104167,
                        "dayId": 119104168,
                        "itemId": 119104173
                    }
                },
                {
                    "name": "Tagliatelle s kuøecím masem a rajèaty",
                    "allergens": "1a,3,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119104167,
                        "dayId": 119104168,
                        "itemId": 119104174
                    }
                }
            ]
        }
    ]
}
```
> Returns all the menus, that P# is aware of.
So the menus for this week and possibly for another. 
If there is no menu, an empty array.

>One menu belongs to one day. A Menu has multiple foods (3) in it.
