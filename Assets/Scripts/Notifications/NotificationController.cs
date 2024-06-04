using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationController : MonoBehaviour
{
    #if UNITY_IOS
    [SerializeField] private iOSNotifications iosNotifications; 

    private void Start()
    {
        StartCoroutine(iosNotifications.RequestAuthorization());
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            iosNotifications.SendNotification("SUPERTHIEF", "Pablo Escobar has invited you to a team heist. Join now and plan the perfect heist together!", "Team Heist", 59);
        }
    }
    #endif
}