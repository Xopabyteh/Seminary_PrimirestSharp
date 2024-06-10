using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Yearly.Contracts;
using Yearly.Contracts.Common;
using Yearly.MauiClient.Resources;

namespace Yearly.MauiClient.Services;

public static class ProblemToLocalizedMessage
{
    public static string GetLocalizedMessage(this ProblemDetails problem)
    {
        // The problem details type is a little misfortunate with its .Extensions,
        // what happens here is errorCodesObj is a JsonElement with ValueKind.Array
        if (!problem.Extensions.TryGetValue(ProblemDetailsExtensionKeys.ErrorCodes, out var errorCodesObj))
        {
            return problem.Title is not null
                ? UnlocalizedMarkedTitle(problem.Title)
                : LocalizationResources.ResourceManager.GetString(ErrorCodes.UnknownError)!;
        }

        // Cast the error codes to a JsonElement and extract the strings
        var errorCodesJsonElement = (JsonElement)errorCodesObj!;
        var errorCodes = errorCodesJsonElement
            .EnumerateArray()
            .Select(e => e.GetString());

        // Show the first error code
        var firstErrorCode = errorCodes.FirstOrDefault();
        if (firstErrorCode is not null)
        {
            var localizedMessage = LocalizationResources.ResourceManager.GetString(firstErrorCode);
            if (localizedMessage is not null)
            {
                return localizedMessage;
            }
        }

        // -> No error codes attached
        return problem.Title is not null
            ? UnlocalizedMarkedTitle(problem.Title) 
            : LocalizationResources.ResourceManager.GetString(ErrorCodes.UnknownError)!;
    }

    private static string UnlocalizedMarkedTitle(string title)
        => $"*{title}"; // Add star to show that it's not localized
}