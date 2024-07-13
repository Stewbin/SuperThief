using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    [Header("Screens")]
    public GameObject loadingScreen;
    public GameObject lobbyScreen;
    public GameObject roomScreen;
    public GameObject errorScreen;
    public GameObject roomBrowserScreen;

    [Header("Loading Screen")]
    public TMP_Text loadingText;

    [Header("Lobby Screen")]
    public Button findRoomButton;
    public Button createRoomButton;
    public Button quickJoinButton;
    public Button singlePlayerButton;

    [Header("Room Screen")]
    public TMP_Text roomNameText;
    public TMP_Text playerListText;

    [Header("Error Screen")]
    public TMP_Text errorText;

    [Header("Room Browser Screen")]
    public RoomButton theRoomButton;
    public List<RoomButton> allRoomButtons = new List<RoomButton>();

 public TMP_Text playerNameLabel; 
   private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public string levelToPlay;
    public GameObject startButton;
    public GameObject roomTestButton;

    private const string RoomNameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int RoomNameLength = 6;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CloseAllScreens();
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to server...";

        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void CloseAllScreens()
    {
        loadingScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        loadingText.text = "Joining Lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        CloseAllScreens();
        lobbyScreen.SetActive(true);
        Debug.Log("Joined Lobby");

        PhotonNetwork.NickName = PlayerPrefs.GetString("USERNAME");

         print("You have succesfully connected to a Photon Lobby");

    }

    public void CreateRoom()
    {
        string randomRoomName = GenerateRandomRoomName();
        RoomOptions options = new RoomOptions { MaxPlayers = 8 };
        PhotonNetwork.CreateRoom(randomRoomName, options);

        CloseAllScreens();
        loadingText.text = "Creating room...";
        loadingScreen.SetActive(true);
    }

    private string GenerateRandomRoomName()
    {
        char[] roomName = new char[RoomNameLength];
        for (int i = 0; i < RoomNameLength; i++)
        {
            roomName[i] = RoomNameChars[Random.Range(0, RoomNameChars.Length)];
        }
        return new string(roomName);
    }

    public void FindRoom()
    {
        CloseAllScreens();
        roomBrowserScreen.SetActive(true);
    }

    public void JoinRandomRoom()
    {
        CloseAllScreens();
        loadingText.text = "Joining random room...";
        loadingScreen.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartSinglePlayerGame()
    {
        SceneManager.LoadScene("SinglePlayerScene");
    }

    public override void OnJoinedRoom()
    {
        CloseAllScreens();
        roomScreen.SetActive(true);
        roomNameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        /// <summary>
        /// playerListText.text = "";
        //foreach (Player player in PhotonNetwork.PlayerList)
       // {
        //    playerListText.text += player.NickName + "\n";
       // }
        /// </summary>
        /// /// 
       
       foreach(TMP_Text player in allPlayerNames){

            Destroy(player.gameObject);
        }

        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList; 


        for (int i = 0; i < players.Length; i++){
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel,playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true); 

            allPlayerNames.Add(newPlayerLabel);
        }   
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        CloseAllScreens();
        errorScreen.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        errorText.text = "No rooms available. Creating a new room.";
        CreateRoom();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseAllScreens();
        loadingText.text = "Leaving Room...";
        loadingScreen.SetActive(true);

        lobbyScreen.SetActive(true);


    }

    public override void OnLeftRoom()
    {
        CloseAllScreens();
        lobbyScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        CloseAllScreens();
        loadingText.text = "Joining Room";
        loadingScreen.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }

        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                newButton.SetButtonDetails(roomList[i]);
                newButton.gameObject.SetActive(true);
                allRoomButtons.Add(newButton);
            }
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public void SetNickname()
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("USERNAME");
    }

     public void CloseRoomBrowser(){
        CloseAllScreens(); 
        roomBrowserScreen.SetActive(false);
        lobbyScreen.SetActive(true);
        


    }
}