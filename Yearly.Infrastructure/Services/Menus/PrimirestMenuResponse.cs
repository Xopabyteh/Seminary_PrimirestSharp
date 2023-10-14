﻿// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public record PrimirestMenuResponseDay(
    IReadOnlyList<PrimirestMenuResponseItem> Items,
    DateTime Date
);

public record PrimirestMenuResponseItem(
    bool CanOrder,
    string Description,
    //DateTime MenuDayDate,
    //IReadOnlyList<MealAllergensList> MealAllergensList,
    string MealAllergensMarkings
);


//public record MealAllergensList(
//int Item1,
//string Item2,
//string Item3,
//int Item4,
//int Item5
//);

public record PrimirestMenuResponseMenu(
    IReadOnlyList<object> Orders,
    IReadOnlyList<PrimirestMenuResponseDay> Days

    //DateTime DateFrom,
    //DateTime DateTo,
);

public record PrimirestMenuResponseRoot(
    PrimirestMenuResponseMenu Menu
);
