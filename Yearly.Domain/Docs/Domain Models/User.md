## User

```csharp
	void OrderFood(Food);

	void PublishPhoto(Photo);

	void AprovePhoto(PhotoId); // Admin only
```

```json
{
	"id": {"value": "0"}
	"username": "Martin Fiala",
	"roles": [
		"User",
		"Admin"
	],
	"photoIds": [
		{"value": "00000000-0000-0000-0000-000000000000"}
	]
}
```

> id is not a guid, but an integer, because primirest uses integers for ids and intergration is easier that way