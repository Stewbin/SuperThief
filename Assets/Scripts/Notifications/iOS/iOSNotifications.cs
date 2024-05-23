using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using Unity.Notifications.iOS; 
#endif


public class iOSNotifications : MonoBehaviour
{

#if UNITY_IOS
    public IEnumerator RequestAuthorization()
    {
        using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            yield return null;
        }
        
        /// You can check the result of the authorization request here
        if (request.Granted)
        {
            Debug.Log("Notification authorization granted.");
        }
        else
        {
            Debug.LogWarning("Notification authorization denied.");
        }
    }

    // Setup and send notification
    public void SendNotification(string title, string body, string subtitle, int fireTimeInMinutes)
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(0, fireTimeInMinutes, 0),
            Repeats = true
        };

        var notification = new iOSNotification()
        {
            Identifier = "Testing",
            Title = title,
            Body = body,
            Subtitle = subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Badge),
            CategoryIdentifier = "default_category",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }

    #endif
}
