#if DEBUG
using Microsoft.AspNetCore.Components;
using Yearly.Infrastructure.Persistence.Seeding;
#endif

namespace Yearly.Presentation.Pages;

public partial class DevPage
{
#if DEBUG
    /// <summary>
    /// I Know, injecting a db context into a server side blazor app
    /// is madness, but this is only for few development purposes anyway
    /// </summary>
    [Inject] private DataSeeder _dataSeeder { get; set; } = null!; 
#endif

    public async Task DbReset()
    {
#if DEBUG
        await _dataSeeder.DbResetAsync();
#endif
    }

}