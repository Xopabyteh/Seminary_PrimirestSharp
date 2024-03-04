#if ANDROID
using Java.Lang;
using WindowsAzure.Messaging.NotificationHubs;
using Object = Java.Lang.Object;
#endif

namespace Yearly.MauiClient.Services;

public class IndependentNotificationHubService
{
    public void AddTag(string tag)
    {
#if ANDROID
        NotificationHub.AddTag(tag);
#endif
        
        throw new NotImplementedException();
    }

    public void RemoveTag(string tag)
    {
#if ANDROID
        NotificationHub.RemoveTag(tag);
#endif

        throw new NotImplementedException();
    }

    public string[] GetTags()
    {
#if ANDROID
        IIterable tags = NotificationHub.Tags;
        var consumer = new TagConsumer();
        tags.ForEach(consumer);

        return consumer.Result.ToArray();
#endif

        throw new NotImplementedException();
    }

#if ANDROID
    private class TagConsumer : Java.Lang.Object, Java.Util.Functions.IConsumer
    {
        public List<string> Result { get; private set; } = new(4);
        public void Accept(Object? t)
        {
            string tag = t!.ToString();
            Result.Add(tag);
        }
    }
#endif
}