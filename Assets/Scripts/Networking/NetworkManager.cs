using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{

     void Start()
    {
        
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
     
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
       
    }

    public override void OnJoinedLobby()
    {
        print("Joined Lobby");
    }

}