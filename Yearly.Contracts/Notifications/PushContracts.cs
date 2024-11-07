namespace Yearly.Contracts.Notifications;

public partial class PushContracts
{
    public static string CreateTopic(string name, int notificationId)
        => $"{name}_{notificationId}";

    /// <summary>
    /// Not secure in any way, anyone can listen to this!!!
    /// </summary>
    /// <param name="name"></param>
    /// <param name="notificationId"></param>
    /// <param name="userId"></param>
    public static string CreateTopic(string name, int notificationId, int userId)
        => $"{name}_{userId}_{notificationId}";
}