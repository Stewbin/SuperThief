using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    [Header("Screens")]
    public GameObject loadingScreen;
    public GameObject lobbyScreen;
    public GameObject roomScreen;
    public GameObject errorScreen;
    public GameObject roomBrowserScreen;
    public GameObject loadingGameScreen;

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
    public TMP_Text message;

    [Header("Error Screen")]
    public TMP_Text errorText;

    [Header("Room Browser Screen")]
    public RoomButton theRoomButton;
    public List<RoomButton> allRoomButtons = new List<RoomButton>();
    public Button refreshButton;
    public TMP_Text refreshingText;

    public TMP_Text playerNameLabel;
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public string levelToPlay;
    public GameObject startButton;
    public GameObject roomTestButton;

    private const string RoomNameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int RoomNameLength = 6;
    private const string GameStartedKey = "gameStarted";

    void Awake()
    {
        instance = this;
        loadingGameScreen.SetActive(false);
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

        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshRoomList);
        }
        else
        {
            Debug.LogWarning("Refresh button is not assigned in the Inspector.");
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

        print("You have successfully connected to a Photon Lobby");
    }

    public void CreateRoom()
    {
        string randomRoomName = GenerateRandomRoomName();
        RoomOptions options = new RoomOptions { 
            MaxPlayers = 8,
            CustomRoomProperties = new PhotonHashtable() { {GameStartedKey, false} },
            CustomRoomPropertiesForLobby = new string[] { GameStartedKey }
        };
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
        PhotonHashtable expectedCustomRoomProperties = new PhotonHashtable() { {GameStartedKey, false} };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);

        CloseAllScreens();
        loadingText.text = "Joining random room...";
        loadingScreen.SetActive(true);
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
        foreach(TMP_Text player in allPlayerNames){
            Destroy(player.gameObject);
        }

        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        
        System.Array.Sort(players, (a, b) => a.ActorNumber.CompareTo(b.ActorNumber));

        for (int i = 0; i < players.Length; i++){
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true); 

            allPlayerNames.Add(newPlayerLabel);
        }   
        
        UpdateStartButtonVisibility();
    }

    void UpdateStartButtonVisibility()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            startButton.SetActive(true);
            message.gameObject.SetActive(false); 
        }
        else
        {
            startButton.SetActive(false);
            message.text = "Cannot start game. Either not Master Client or not enough players."; 
            message.gameObject.SetActive(true); 
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
        if (info.CustomProperties.TryGetValue(GameStartedKey, out object gameStarted) && (bool)gameStarted)
        {
            Debug.Log("Cannot join room. Game has already started.");
            return;
        }

        PhotonNetwork.JoinRoom(info.Name);
        CloseAllScreens();
        loadingText.text = "Joining Room";
        loadingScreen.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (refreshingText != null)
        {
            refreshingText.gameObject.SetActive(false);
        }

        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList || !info.IsOpen || !info.IsVisible || info.PlayerCount >= info.MaxPlayers)
            {
                RoomButton existingButton = allRoomButtons.Find(x => x.info.Name == info.Name);
                if (existingButton != null)
                {
                    Destroy(existingButton.gameObject);
                    allRoomButtons.Remove(existingButton);
                }
            }
            else
            {
                if (info.CustomProperties.TryGetValue(GameStartedKey, out object gameStarted) && !(bool)gameStarted)
                {
                    RoomButton existingButton = allRoomButtons.Find(x => x.info.Name == info.Name);
                    if (existingButton != null)
                    {
                        existingButton.SetButtonDetails(info);
                    }
                    else
                    {
                        RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                        newButton.SetButtonDetails(info);
                        newButton.gameObject.SetActive(true);
                        allRoomButtons.Add(newButton);
                    }
                }
            }
        }

        allRoomButtons.Sort((a, b) => string.Compare(a.info.Name, b.info.Name));

        for (int i = 0; i < allRoomButtons.Count; i++)
        {
            allRoomButtons[i].transform.SetSiblingIndex(i);
        }

        theRoomButton.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new PhotonHashtable() { {GameStartedKey, true} });
            PhotonNetwork.LoadLevel(levelToPlay);
            loadingGameScreen.SetActive(true);
        }
        else
        {
            Debug.Log("Cannot start game. Either not Master Client or not enough players.");
            loadingGameScreen.SetActive(false);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateStartButtonVisibility();
    }

    public void SetNickname()
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("USERNAME");
    }

    public void CloseRoomBrowser()
    {
        CloseAllScreens(); 
        lobbyScreen.SetActive(true);
    }

    public void RefreshRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            if (refreshingText != null)
            {
                refreshingText.gameObject.SetActive(true);
            }
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.LogWarning("Cannot refresh room list. Not din lobby.");
        }
    }
}