namespace Yearly.Infrastructure.Services.Orders.ResponseModels;

//public record HatchType(
//    int ID,
//    object Code,
//    string Value,
//    bool IsReadOnly,
//    string Title
//);

//public record PurchasePlace(
//    bool AllowOrders,
//    bool DisableOrdersMulti,
//    object AllowedGroups,
//    int ID,
//    object Code,
//    string Value,
//    bool IsReadOnly,
//    string Title
//);

public record PrimirestOrderSummaryResponseRoot(
    //object IDPurchasePlace,
    //IReadOnlyList<PurchasePlace> PurchasePlaces,
    //IReadOnlyList<HatchType> HatchTypes,
    IReadOnlyList<Row> Rows
);

public record Row(
    //string PLU,
    //string BoarderNumber,
    //string Boarder,
    //int Order,
    //int OrderItem,
    //string Date,
    //string PurchasePlace,
    //string HatchType,
    //string ProductType,
    //string ProductGroup,
    //string Description,
    //string MealColour,
    //int Energy,
    //int Qty,
    decimal UnitPrice
    //int Dispensed,
    //int Consumed,
    //int Exchanged,
    //int TotalPrice,
    //bool AlternateRow,
    //bool CanCancel,
    //string ActionInfo,
    //int IDModifier,
    //string ModifierName,
    //string ModificationTime
);

