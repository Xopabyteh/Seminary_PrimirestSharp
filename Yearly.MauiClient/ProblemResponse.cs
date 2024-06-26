namespace Yearly.MauiClient;

public readonly record struct ProblemResponse(
    string Title,
    int Status,
    IReadOnlyList<string> ErrorCodes);