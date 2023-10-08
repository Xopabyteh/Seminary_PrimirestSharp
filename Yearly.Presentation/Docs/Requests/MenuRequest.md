## Get Menu

```json
GET {{host}}/menu?sessionCookie={{sessionCookie}}
```


### Response
```json
{
    "menus": [
        {
            "date": "2019-11-11T00:00:00",
            "foods": [
                {
                    "name": "Obalovaná treska, vaøené brambory",
                    "allergens": "1a, 7, 9",
                    "images": [
                        "https:azure-storage/treska249204",
                        "https:azure-storage/treska249204"
                    ]
                }
            ]
        }
    ]
}
```
> Returns all the menus, that P# is aware of.
So the menus for this week and possibly for another. 
If there is no menu, an empty array.

>One menu belongs to one day. A Menu has multiple foods in it.
