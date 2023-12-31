namespace Yearly.Infrastructure.Services.Orders.ResponseModels;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public record BalanceResponseRoot(
    IReadOnlyList<Rows2> Rows2 //There is always only 1 element here, unbelievable
);


public record Rows2(
    //decimal InitialBalance,
    //decimal Incomes,
    //decimal Withdrawals,
    //decimal Consumptions,
    //decimal Reservations,
    //decimal ServiceTransactions,
    decimal ClosingBalance //"Stav Konta"
);

