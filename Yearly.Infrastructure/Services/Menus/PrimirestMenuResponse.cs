// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

internal record PrimirestMenuResponseDay(
    IReadOnlyList<PrimirestMenuResponseItem> Items,
    DateTime Date
);

internal record PrimirestMenuResponseItem(
    bool CanOrder,
    string Description,
    string MealAllergensMarkings,

    //For order identifier      (their naming is confusing)
    int IDMenuDay, //dayID
    int IDMenu, //menuID
    int ID //itemID
);

// public record MealAllergensList(
// int Item1,
// string Item2,
// string Item3,
// int Item4,
// int Item5
// );

internal record PrimirestMenuResponseMenu(
    IReadOnlyList<PrimirestMenuResponseOrder> Orders,
    IReadOnlyList<PrimirestMenuResponseDay> Days

    // DateTime DateFrom,
    // DateTime DateTo,
);

internal record PrimirestMenuResponseOrder(
    IReadOnlyList<PrimirestMenuResponseOrderItem> Items
);

internal record PrimirestMenuResponseOrderItem(
    int ID, //ID of the order item
    int IDOrder, //ID of the order
    int IDItem //ID of the food, the naming is horrible
);

internal record PrimirestMenuResponseRoot(
    PrimirestMenuResponseMenu Menu
);
