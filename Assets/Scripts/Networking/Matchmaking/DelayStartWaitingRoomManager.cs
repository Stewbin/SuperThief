using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DelayStartWaitingRoomManager : MonoBehaviourPunCallbacks
{
    private PhotonView myPhotonView;
    [SerializeField] private int menuSceneIndex;
    [SerializeField] private int multiplayerSceneIndex;

    [SerializeField] private int playerCount;
    [SerializeField] private int minPlayersToStart;

    [SerializeField] private TMP_Text roomCountDisplay;
    [SerializeField] private TMP_Text playerCountDisplay;
    [SerializeField] private TMP_Text timerToStartDisplay;

    [SerializeField] private int roomSize;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    public TMP_Text playerNameLabel ;

    public TMP_Text playerLeftOrJoinText; 
   private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    [SerializeField] private float maxWaitTime;
    [SerializeField] private float maxFullGameWaitTime;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        PlayerCountUpdate();
    }

    public void PlayerCountUpdate()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomCountDisplay.text = playerCount + "/" + roomSize;

        if (playerCount == roomSize)
        {
            readyToStart = true;
        }
        else if (playerCount >= minPlayersToStart)
        {
            readyToCountDown = true;
        }
        else
        {
            readyToCountDown = false;
            readyToStart = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();

            //Added player Nickname UI

            string savedUsername = PlayerPrefs.GetString("USERNAME");
            PhotonNetwork.NickName = PlayerPrefs.GetString("USERNAME");

            newPlayer.NickName = PlayerPrefs.GetString("USERNAME"); 
    
            if (playerNameLabel != null)
            {
        playerNameLabel.text = PlayerPrefs.GetString("USERNAME");
        }
        else
        {
         print("playerNameLabel is not assigned!");
        }
           
            //Added player Nickname UI


        if (PhotonNetwork.IsMasterClient)
        {
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);
        }

        print("This player has joined the room" + newPlayer.NickName); 
        StartCoroutine(DisplayPlayerJointUI(newPlayer.NickName)); 
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn)
    {
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;

        if (timeIn < fullGameTimer)
        {
            fullGameTimer = timeIn;
        }
    }

public override void OnPlayerLeftRoom(Player otherPlayer)
{
    PlayerCountUpdate();
    StartCoroutine(DisplayPlayerLeftUI(otherPlayer.NickName));
}

IEnumerator DisplayPlayerLeftUI(string playerName)
{
    playerLeftOrJoinText.gameObject.SetActive(true);
    playerLeftOrJoinText.text = playerName + " has left the room";
    yield return new WaitForSeconds(3f);
    playerLeftOrJoinText.gameObject.SetActive(false);
}

IEnumerator DisplayPlayerJointUI(string playerName)
{
    playerLeftOrJoinText.gameObject.SetActive(true);
    playerLeftOrJoinText.text = playerName + " has joined the room";
    yield return new WaitForSeconds(3f);
    playerLeftOrJoinText.gameObject.SetActive(false);
}



    private void Update()
    {
        WaitingForMorePlayers();
    }

    public void WaitingForMorePlayers()
    {
        if (playerCount <= 1)
        {
            ResetTimer();
        }

        if (readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if (readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }
        else
        {
            ResetTimer(); // Reset the timer if not ready to count down
        }

        timerToStartDisplay.text = timerToStartGame.ToString("00");

        if (timerToStartGame <= 0f)
        {
            if (startingGame)
            {
                return;
            }
            StartGame();
        }
    }

    public void ResetTimer()
    {
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    public void StartGame()
    {
        startingGame = true;

    

        // Ensure all players are ready before starting the game
        if (PhotonNetwork.PlayerList.Length >= minPlayersToStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
        else
        {
            // If not enough players are present, reset the starting game flag
            startingGame = false;
        }
    }

    public void DelayCancel()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }

  public void SetNickname()
{
    string savedUsername = PlayerPrefs.GetString("USERNAME");
    PhotonNetwork.NickName = PlayerPrefs.GetString("USERNAME");
    
    if (playerNameLabel != null)
    {
        playerNameLabel.text = PlayerPrefs.GetString("USERNAME");
    }
    else
    {
       print("playerNameLabel is not assigned!");
    }
}
}
