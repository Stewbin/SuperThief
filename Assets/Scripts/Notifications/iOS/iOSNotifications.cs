using System.Collections;
using Unity.Notifications.iOS;
using UnityEngine;
public class iOSNotifications : MonoBehaviour
{
#if UNITY_IOS
    private readonly string[] notificationMessages = new string[]
    {
        "Gear up, thief! It's time to dominate the heist!",
        "Ready to steal? Prove you're the ultimate thief!",
        "No mercy. No limits. Claim your loot now!",
        "The heist is live. Show no mercy and take it all!",
        "Time to wreak havoc and snatch the cash!",
        "Unleash chaos. The ultimate heist starts now!",
        "Be ruthless. Be relentless. Win the heist!",
        "The vault is open. Crush your rivals and loot big!",
        "Show your dominance. The heist is on!",
        "Strike hard, steal fast. The heist is yours!"
    };

    public IEnumerator RequestAuthorization()
    {
        Debug.Log("Requesting notification authorization...");
        using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            yield return null;
        }

        if (request.Granted)
        {
            Debug.Log("Notification authorization granted.");
        }
        else
        {
            Debug.LogWarning("Notification authorization denied.");
        }
    }

    public void SendNotification()
    {
        Debug.Log("Sending notification...");
        string randomMessage = notificationMessages[Random.Range(0, notificationMessages.Length)];
        Debug.Log("Notification message: " + randomMessage);

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(0, 0, 10), // 10 seconds for testing
            Repeats = true
        };

        var notification = new iOSNotification()
        {
            Identifier = "_6HourNotification",
            Body = randomMessage,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Badge),
            CategoryIdentifier = "default_category",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);
        Debug.Log("Notification scheduled.");
    }
#endif
}
