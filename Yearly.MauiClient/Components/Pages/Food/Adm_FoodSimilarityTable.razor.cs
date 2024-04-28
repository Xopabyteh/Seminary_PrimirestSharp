using Microsoft.AspNetCore.Components;
using Yearly.Contracts.Foods;
using Yearly.MauiClient.Services.SharpApi.Facades;

namespace Yearly.MauiClient.Components.Pages.Food;

public partial class Adm_FoodSimilarityTable
{
    [Inject] public FoodFacade FoodFacade { get; set; } = null!;
    private List<FoodSimilarityRecordDTO> similarityRecords = new();
    private Dictionary<FoodSimilarityRecordDTO, RecordResolveState> similarityRecordResolveStates = new();

    private bool recordsLoaded = false;
    protected override async Task OnInitializedAsync()
    {
        var similarityTable = await FoodFacade.GetSimilarityTableAsync();
        similarityRecords = similarityTable.Records;

        foreach (var record in similarityRecords)
        {
            similarityRecordResolveStates.Add(record, RecordResolveState.Unresolved);
        }

        recordsLoaded = true;
    }

    private async Task ApproveRecord(FoodSimilarityRecordDTO record)
    {
        await FoodFacade.ApproveSimilarityAsync(
            record.NewlyPersistedFood.Id,
            record.PotentialAlias.Id);

        similarityRecordResolveStates[record] = RecordResolveState.Approved;
        StateHasChanged();
    }

    private async Task DisapproveRecord(FoodSimilarityRecordDTO record)
    {
        await FoodFacade.DisapproveSimilarityAsync(
            record.NewlyPersistedFood.Id,
            record.PotentialAlias.Id);

        similarityRecordResolveStates[record] = RecordResolveState.Disapproved;
        StateHasChanged();
    }

    private enum RecordResolveState
    {
        Unresolved,
        Disapproved,
        Approved
    }
}