//This file contains necessary models for correct deserialization from the Primirest API to objects

using Yearly.Infrastructure.Services.Menus;

internal record PrimirestOrderResponseRoot(
    bool Success,
    string? Message,
    IReadOnlyList<PrimirestMenuResponseOrder> Orders
);
