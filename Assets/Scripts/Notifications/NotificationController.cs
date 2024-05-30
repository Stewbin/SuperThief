using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationController : MonoBehaviour
{
    [SerializeField] private iOSNotifications iosNotifications; 

    private void Start()
    {
        StartCoroutine(iosNotifications.RequestAuthorization());
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            iosNotifications.SendNotification("SUPERTHIEF", "We are cooking babyyyy!!!!", "Lunexis", 59);
        }
    }
}