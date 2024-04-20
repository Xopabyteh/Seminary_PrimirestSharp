namespace Yearly.Contracts.Common;

/// <summary>
/// Represents a fragment of data after filtering
/// </summary>
/// <param name="Data">The data, if pagination was used as a filter it might not be all available data at once</param>
/// <param name="TotalCount">The count of all data that could be retrieved</param>
public record DataFragmentDTO<T>(
    IList<T> Data,
    int TotalCount);