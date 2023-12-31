﻿using ErrorOr;

namespace Yearly.Infrastructure.Errors;

public static partial class Errors
{
    public static class PrimirestAdapter
    {
        public static Error InvalidAdminCredentials
            => Error.Validation("PrimirestAdapter.Credentials", "The admin credentials are incorrect");
    }
}