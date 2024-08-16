using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class DelayStartWaitingRoomManager : MonoBehaviourPunCallbacks
{
    private PhotonView myPhotonView;
    [SerializeField] private int menuSceneIndex;
    [SerializeField] private int multiplayerSceneIndex;

    [SerializeField] private int playerCount;
    [SerializeField] private int minPlayersToStart;

    [SerializeField] private TMP_Text roomCountDisplay;
    [SerializeField] private TMP_Text timerToStartDisplay;

    [SerializeField] private int roomSize;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    public TMP_Text playerNameLabelPrefab;
    public Transform playerListContent;
    public TMP_Text playerLeftOrJoinText;
    private Dictionary<int, TMP_Text> playerListItems = new Dictionary<int, TMP_Text>();

    [SerializeField] private float maxWaitTime;
    [SerializeField] private float maxFullGameWaitTime;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        PlayerCountUpdate();
        SetNickname();
        UpdatePlayerList();
    }

    public void PlayerCountUpdate()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomCountDisplay.text = $"{playerCount}/{roomSize}";

        bool shouldStartCountdown = playerCount >= minPlayersToStart;
        if (PhotonNetwork.IsMasterClient)
        {
            myPhotonView.RPC("RPC_UpdateCountdownStatus", RpcTarget.All, shouldStartCountdown, playerCount == roomSize);
        }
    }

    [PunRPC]
    private void RPC_UpdateCountdownStatus(bool countDown, bool startNow)
    {
        readyToCountDown = countDown;
        readyToStart = startNow;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();
        UpdatePlayerList();

        if (PhotonNetwork.IsMasterClient)
        {
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.All, timerToStartGame);
        }

        StartCoroutine(DisplayPlayerJointUI(newPlayer.NickName));
        Debug.Log($"Player entered: {newPlayer.NickName} (ActorNumber: {newPlayer.ActorNumber})");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
        UpdatePlayerList();
        StartCoroutine(DisplayPlayerLeftUI(otherPlayer.NickName));
        Debug.Log($"Player left: {otherPlayer.NickName} (ActorNumber: {otherPlayer.ActorNumber})");
    }

    private void UpdatePlayerList()
    {
        Debug.Log("Updating player list...");

        // Remove players that have left the room
        List<int> playersToRemove = new List<int>();
        foreach (var kvp in playerListItems)
        {
            if (PhotonNetwork.PlayerList.All(p => p.ActorNumber != kvp.Key))
            {
                Destroy(kvp.Value.gameObject);
                playersToRemove.Add(kvp.Key);
                Debug.Log($"Removing player from list: ActorNumber {kvp.Key}");
            }
        }
        foreach (int key in playersToRemove)
        {
            playerListItems.Remove(key);
        }

        // Add new players or update existing ones
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!playerListItems.TryGetValue(player.ActorNumber, out TMP_Text playerLabel))
            {
                playerLabel = Instantiate(playerNameLabelPrefab, playerListContent);
                playerListItems.Add(player.ActorNumber, playerLabel);
                Debug.Log($"Adding new player to list: {player.NickName} (ActorNumber: {player.ActorNumber})");
            }
            playerLabel.text = player.NickName;
        }

        // Debug log
        Debug.Log($"Players in room: {string.Join(", ", PhotonNetwork.PlayerList.Select(p => $"{p.NickName} ({p.ActorNumber})"))}");
        Debug.Log($"Players in list: {string.Join(", ", playerListItems.Select(kvp => $"{kvp.Value.text} ({kvp.Key})"))}");
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn)
    {
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        fullGameTimer = timeIn;
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
            return;
        }

        if (readyToStart || readyToCountDown)
        {
            timerToStartGame -= Time.deltaTime;
            timerToStartGame = Mathf.Max(timerToStartGame, 0f);
           
            if (PhotonNetwork.IsMasterClient)
            {
                myPhotonView.RPC("RPC_SendTimer", RpcTarget.All, timerToStartGame);
            }
        }
        else
        {
            ResetTimer();
        }

        timerToStartDisplay.text = timerToStartGame.ToString("00");

        if (timerToStartGame == 0f && !startingGame)
        {
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
        if (PhotonNetwork.IsMasterClient)
        {
            myPhotonView.RPC("RPC_StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        startingGame = true;
        if (PhotonNetwork.PlayerList.Length >= minPlayersToStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
        else
        {
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
        PhotonNetwork.NickName = savedUsername;
    }
}