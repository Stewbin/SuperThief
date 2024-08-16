using System.Collections;
using UnityEngine;


public class NotificationController : MonoBehaviour
{
#if UNITY_IOS
    [SerializeField] private iOSNotifications iosNotifications;

    private void Start()
    {
        Debug.Log("NotificationController Start method called.");
        StartCoroutine(iosNotifications.RequestAuthorization());
    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("OnApplicationFocus called with focus: " + focus);
        if (!focus)
        {
            iosNotifications.SendNotification();
        }
    }
#endif
}
