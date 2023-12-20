namespace Yearly.MauiClient.Exceptions;

public readonly record struct ProblemResponse(
    string Title,
    int Status,
    IReadOnlyList<string> ErrorCodes);