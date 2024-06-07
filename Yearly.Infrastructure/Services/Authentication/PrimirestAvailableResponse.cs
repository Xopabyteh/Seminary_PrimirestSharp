using Newtonsoft.Json;

namespace Yearly.Infrastructure.Services.Authentication;

/// <summary>
/// Even tough the name is leading, this is the response from the available endpoint,
/// which returns all available users within the "user tenant"
/// </summary>

public record AvailableResponseRoot(
    IReadOnlyList<AvailableResponseItem> Items
);

/// <param name="ID"></param>
/// <param name="Name"></param>
/// <param name="AdditionalInfo">E.X. "B8609A8; sk. 3 (studenti 15 a více let)"</param>
public record AvailableResponseItem(
    //bool _suppressReadOnly,
    int ID,
    //int IDEntityType,
    //int IDState,
    //int IDParent,
    //int IDEntityTypeParent,
    string Name,
    //string Number,
    //string NameParent,
    //string NumberParent,
    string AdditionalInfo
    //string Key,
    //bool IsNew,
    //bool IsDeleted,
    //bool IsDirty,
    //bool IsValid,
    //bool IsReadOnly,
    //bool SuppressReadOnly,
    //bool HasErrors,
    //string Error,
    //object ChildName
);

