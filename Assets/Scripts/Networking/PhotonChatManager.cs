/*using Photon.Chat;
using UnityEngine;
using Photon.Pun;
using System;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string nickName;
    private ChatClient chatClient;

    private void Awake()
    {
        nickName = PlayerPrefs.GetString("USERNAME");
    }

    private void Start()
    {
        chatClient = new ChatClient(this);
        ConnectToPhotonChat();
    }

    private void Update()
    {
        chatClient.Service();
    }

    private void ConnectToPhotonChat()
    {
        print("Connecting to PhotonChat...");
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(nickName);
        ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.ConnectUsingSettings(chatSettings);
    }

    public void OnConnected()
    {
        print("Successfully connected to Photon Chat");
        SendDirectMessage("Ash", "Hey Ash chat is working");
    }

    public void OnDisconnected()
    {
        print("Disconnected from Photon Chat. Cause: " + cause);
    }

    public void OnChatStateChange(ChatState state)
    {
        print("Chat state changed: " + state);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }

    public void SendDirectMessage(string recipient, string message)
    {
        chatClient.SendPrivateMessage(recipient, message);
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (!string.IsNullOrEmpty(message.ToString()))
        {
            // Channel Name format: [Sender:Recipient]
            string[] splitNames = channelName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (splitNames.Length == 2)
            {
                string senderName = splitNames[0].Trim();
                string recipientName = splitNames[1].Trim();

                if (!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
                {
                    print($"Private message from {sender} to {recipientName}: {message}");
                }
            }
            else
            {
                print($"Invalid channel name format: {channelName}");
            }
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
      
    }

    public void OnUnsubscribed(string[] channels)
    {
      
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        print("Debug message (" + level + "): " + message);
    }
   
}
*/