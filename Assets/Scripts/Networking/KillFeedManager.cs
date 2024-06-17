using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class KillFeedManager : MonoBehaviourPunCallbacks
{
    

    [PunRPC]
    public void UpdateKillFeed(string killerName, string victimName)
    {
        string killFeedMessage = killerName + " killed " + victimName;

        // Update the kill feed UI
        UIController.instance.killFeedText.text += killFeedMessage + "\n";

        // Limit the number of displayed kill feed messages
        string[] killFeedMessages = UIController.instance.killFeedText.text.Split('\n');
        if (killFeedMessages.Length > 5) // Adjust the limit as needed
        {
           UIController.instance.killFeedText.text  = string.Join("\n", killFeedMessages, killFeedMessages.Length - 5, 5);
        }
    }
}