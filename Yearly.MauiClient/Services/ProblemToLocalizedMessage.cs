using Microsoft.AspNetCore.Mvc;
using Yearly.Contracts;
using Yearly.MauiClient.Resources;

namespace Yearly.MauiClient.Services;

public static class ProblemToLocalizedMessage
{
    public static string GetLocalizedMessage(this ProblemResponse problem)
    {
        // Show the first error code
        var firstErrorCode = problem.ErrorCodes.FirstOrDefault();
        if (firstErrorCode is null)
        {
            // -> No error codes attached
            return LocalizationResources.ResourceManager.GetString(ErrorCodes.UnknownError)!;
        }
        
        var localizedMessage = LocalizationResources.ResourceManager.GetString(firstErrorCode);
        return localizedMessage ??
               LocalizationResources.ResourceManager.GetString(ErrorCodes.UnknownError)!;
    }
}