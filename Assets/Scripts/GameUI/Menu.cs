using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Menu : MonoBehaviour
{
    [SerializeField] private AndroidNotificationHandler androidNotificationHandler;
    [SerializeField] private IOSNotificationHandler iosNotificationHandler;


    public void Play(){
        //CLick Play to start playing
        SceneManager.LoadScene(1);
        print("Successfully started playing");
    }

    
    void SendNotificationAndroid()
    {

#if UNITY_ANDROID
        androidNotificationHandler.ScheduleNotification(Time.deltaTime);

#elif UNITY_IOS
        iosNotificationHandler.ScheduleNotification(1);
#endif
    }
}
