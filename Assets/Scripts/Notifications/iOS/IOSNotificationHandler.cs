using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using Unity.Notifications.iOS; 
#endif
using UnityEngine;

public class IOSNotificationHandler : MonoBehaviour
{
#if UNITY_IOS
public void ScheduleNotification(int minutes){

iOSNotification = new iOSNotification{

Title = "Test Notification 0",
Subtitle = "Lunexis Team",
Body =  "We are testing push notification in our game!",
ShowInForeground = true; 
ForegroundPresentationOption = (PresentationOption.Alert || PresentationOption.Sound),
CategoryIdentifier = "category_a", 
ThreadIdentifier = "thread1",
Trigger = new iOSNotificationTimeIntervalTrigger{

TimeInterval = new System.TimeSpan(0, minutes, 0), 

Repeats = false
}






};

iOSNotification.ScheduleNotification(notification);


}
#endif
}
