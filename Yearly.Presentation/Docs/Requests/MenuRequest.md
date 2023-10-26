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
                    "name": "Krupoto s pe�enou �ervenou �epou a s�rem typu Feta",
                    "allergens": "1c,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119137656,
                        "dayId": 119137727,
                        "itemId": 119137738
                    }
                },
                {
                    "name": "Zape�en� t�stoviny s mlet�m masem a p�rkem",
                    "allergens": "1a,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119137656,
                        "dayId": 119137727,
                        "itemId": 119137737
                    }
                },
                {
                    "name": "Filet Mahi Mahi s bylinkov�m m�slem, va�en� brambory s petr�elkou",
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
                    "name": "Restovan� zelenina s �ervenou fazol� a jogurtov�m dipem, va�en� brambory",
                    "allergens": "1a,7",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119104167,
                        "dayId": 119104168,
                        "itemId": 119104175
                    }
                },
                {
                    "name": "Vep�ov� k�ta du�en� s kr�movou om��kou se zeleninou (Bratislavsk�), du�en� r��e",
                    "allergens": "1a,7,9,10",
                    "imageLinks": [],
                    "primirestOrderIdentifier": {
                        "menuId": 119104167,
                        "dayId": 119104168,
                        "itemId": 119104173
                    }
                },
                {
                    "name": "Tagliatelle s ku�ec�m masem a raj�aty",
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
