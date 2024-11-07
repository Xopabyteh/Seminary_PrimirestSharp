using Plugin.Firebase.CloudMessaging;

namespace Yearly.MauiClient.Services.Notifications;

/// <summary>
/// Subscribes to push notifications topic and
/// remembers the subscription state
/// </summary>
internal class PushRegistrationService
{
    public async Task SubscribeToTopic(string topic)
    {
        await CrossFirebaseCloudMessaging.Current.SubscribeToTopicAsync(topic);

        var prefKey = CreatePrefey(topic);
        Preferences.Set(prefKey, true);
    }

    public async Task UnsubscribeFromTopic(string topic)
    {
        await CrossFirebaseCloudMessaging.Current.UnsubscribeFromTopicAsync(topic);

        var prefKey = CreatePrefey(topic);
        Preferences.Set(prefKey, false);
    }

    public bool IsSubscribedToTopic(string topic)
    {
        var prefKey = CreatePrefey(topic);
        return Preferences.Get(prefKey, false);
    }

    private string CreatePrefey(string topic)
        => $"fcm_topic_{topic}";
}
