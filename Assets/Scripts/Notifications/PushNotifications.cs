/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;

public class PushNotifications : MonoBehaviour
{
    // Start is called before the first frame update
  public void Start() {
  Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
  Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
}

public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
  UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
}

public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
  UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
}
}
*/