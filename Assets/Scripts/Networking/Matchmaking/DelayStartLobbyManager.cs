using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DelayStartLobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Delay Matchmaking configuration")]
    [SerializeField] public GameObject delayStartButton; // Button used for creating and joining a game
    [SerializeField] public GameObject delayCancelButton; // Button used to stop searching and joining a game

    #region Delay Matchmaking

      public override void OnConnectedToMaster()
    {
        print ("Connected to Master");
       delayStartButton.SetActive(true);
    }


    public void DelayStart()
    {
        delayStartButton.SetActive(false);
        delayCancelButton.SetActive(true);

        PhotonNetwork.JoinRandomRoom(); // Initially Try to join an existing room
        print("Delay matchmaking started");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom(); // Create a room for the match
    }

    public void CreateRoom()
    {
        print("Creating room for delay match now...");
        int randomRoomNumber = UnityEngine.Random.Range(0, 10000); // Creating a random room
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 8 };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        print($"Room successfully created. The room name is Room{randomRoomNumber}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Failed to create room: " + message);
    }

    public void DelayCancel()
    {
        delayCancelButton.SetActive(false);
        delayStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}