internal record PrimirestOrderResponseRoot(
    bool Success,
    string? Message,
    IReadOnlyList<PrimirestMenuResponseOrder> Orders
);
