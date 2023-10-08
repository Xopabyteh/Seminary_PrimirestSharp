## Food

```csharp
	void AddPhoto(PhotoId);
```

```json
{
	"id": {"value" : "00000000-0000-0000-0000-000000000000"},
	"aliasForId?": {"value" : "00000000-0000-0000-0000-000000000000"},
	"photoIds": [
		{ "value" : "00000000-0000-0000-0000-000000000000" }
	],
	"date": "2019-11-11T00:00:00",
	"name": "Obalovan� treska, va�en� brambory",
	"allergens": "1a, 7, 9"
}
```
> a food might have an alias `aliasForId`. When null, there is no alias and the food is original. When there is value, the food already exists and the aliasId is the Id of the original food.