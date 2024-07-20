using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    public void Awake()
    {
        instance = this;
    }

    private enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat,
        NextMatch,
        TimerSync,
        WaitingTimerSync
    }

    public enum GameState
    {
        Waiting,
        Playing,
        Ending
    }

    [Header("Match Settings")]
    public GameState state = GameState.Waiting;
    public float waitAfterEnding = 5f;
    public float waitBeforeBegin = 10f;

    public float matchLength = 60f; 
    public float currentMatchTime; 
    private float sendTimer;
    [SerializeField] public int KILL_REWARD = 25;

    public bool perpetual; 

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;

    private List<Leaderboard> leaderboardPlayers = new List<Leaderboard>();

    private int gamesPlayed = 0;

    private float waitingTimer;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
            state = GameState.Waiting;

            UIController.instance.waitingScreen.SetActive(true);
            waitingTimer = waitBeforeBegin;

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(WaitingTimerCoroutine());
            }

            if(!PhotonNetwork.IsMasterClient)
            {
                UIController.instance.timerText.gameObject.SetActive(false);
            }
        }
    }

    void Update() 
    {
        if(state == GameState.Waiting)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                sendTimer -= Time.deltaTime;
                if(sendTimer <= 0)
                {
                    sendTimer += 1f;
                    WaitingTimerSend();
                }
            }
        }
        else if(state == GameState.Playing)
        {
            if(currentMatchTime > 0f)
            {
                currentMatchTime -= Time.deltaTime; 

                if(currentMatchTime <= 0f)
                {
                    currentMatchTime = 0f;
                    state = GameState.Ending;
                    
                    if(PhotonNetwork.IsMasterClient)
                    {
                        ListPlayersSend();
                        StateCheck();
                    }
                }

                UpdateTimerDisplay();

                if(PhotonNetwork.IsMasterClient)
                {
                    sendTimer -= Time.deltaTime; 

                    if(sendTimer <= 0)
                    {
                        sendTimer += 1f; 
                        TimerSend();  
                    }
                }
            }
        }
    }

    IEnumerator WaitingTimerCoroutine()
    {
        while (waitingTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            waitingTimer--;
            WaitingTimerSend();
        }
        
        state = GameState.Playing;
        UIController.instance.waitingScreen.SetActive(false);
        SetUpTimer();
        ListPlayersSend();
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            switch (theEvent)
            {
                case EventCodes.NewPlayer:
                    NewPlayerReceive(data);
                    break;
                case EventCodes.ListPlayers:
                    ListPlayersReceive(data);
                    break;
                case EventCodes.UpdateStat:
                    UpdateStatsReceived(data);
                    break;
                case EventCodes.NextMatch:
                    NextMatchReceive();
                    break;
                case EventCodes.TimerSync:
                    TimerReceive(data);
                    break;
                case EventCodes.WaitingTimerSync:
                    WaitingTimerReceive(data);
                    break;
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerSend(string username)
    {
        object[] package = new object[5];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;
        package[4] = 0;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }

    public void NewPlayerReceive(object[] dataReceived)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3],(int)dataReceived[4]);
        allPlayers.Add(player);

        ListPlayersSend();
    }

    public void ListPlayersSend()
    {
        object[] package = new object[allPlayers.Count + 1];

        package[0] = state;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[5];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].kills;
            piece[3] = allPlayers[i].deaths;
            piece[4] = allPlayers[i].money;

            package[i + 1] = piece;
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayers,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void ListPlayersReceive(object[] dataReceived)
    {
        allPlayers.Clear();

        state = (GameState)dataReceived[0];

        for (int i = 1; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];

            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3], 
                (int)piece[4] 
            );

            allPlayers.Add(player);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i - 1;
            }
        }

        StateCheck();
    }

    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStat,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void UpdateStatsReceived(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                switch (statType)
                {
                    case 0: // kills
                        allPlayers[i].kills += amount;
                        break;
                    case 1: // deaths
                        allPlayers[i].deaths += amount;
                        break;
                    case 2: // money
                        allPlayers[i].money += amount;
                        break;
                }

                if (i == index)
                {
                    UpdateStatsDisplay();
                }

                if (UIController.instance.leaderboard.activeInHierarchy)
                {
                    ShowLeaderBoard();
                }
                break;
            }
        }
    }

    public void UpdateStatsDisplay()
    {
        if (allPlayers.Count > index)
        {
            UIController.instance.killsText.text = allPlayers[index].kills.ToString();
            UIController.instance.deathsText.text = allPlayers[index].deaths.ToString();
            UIController.instance.moneyText.text = allPlayers[index].money.ToString();
        }
    }

    public void ShowLeaderBoard()
    {
        UIController.instance.leaderboard.SetActive(true);

        foreach (Leaderboard lp in leaderboardPlayers)
        {
            Destroy(lp.gameObject);
        }

        leaderboardPlayers.Clear();
        UIController.instance.leaderboardPlayerDisplay.gameObject.SetActive(false);

        List<PlayerInfo> sorted = SortPlayers(allPlayers);
    
        foreach (PlayerInfo player in sorted)
        {
            Leaderboard newPlayerDisplay = Instantiate(UIController.instance.leaderboardPlayerDisplay, UIController.instance.leaderboardPlayerDisplay.transform.parent);

            newPlayerDisplay.SetDetails(player.name, player.kills, player.deaths, player.money, player.rank);
            newPlayerDisplay.gameObject.SetActive(true);

            leaderboardPlayers.Add(newPlayerDisplay);
        }
    }

    private List<PlayerInfo> SortPlayers(List<PlayerInfo> players)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>(players);
        sorted.Sort((x, y) => y.money.CompareTo(x.money));
        
        // Assign ranks
        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].rank = i + 1;
        }
        
        return sorted;
    }

    public void ToggleLeaderboard()
    {
        if (UIController.instance.leaderboard.activeInHierarchy)
        {
            UIController.instance.leaderboard.SetActive(false);
        }
        else
        {
            ShowLeaderBoard();
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(1);
    }

    void StateCheck()
    {
        if(state == GameState.Ending)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        state = GameState.Ending;

        

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        UIController.instance.endScreen.SetActive(true);

        //deactivate all other UI components 
        UIController.instance.deathScreen.SetActive(false);
        UIController.instance.leaderboardComponent.SetActive(false);
        UIController.instance.healthComponent.SetActive(false);
        UIController.instance.statsComponent.SetActive(false);
        UIController.instance.GunComponent.SetActive(false);
        UIController.instance.timeComponent.SetActive(false);
        UIController.instance.optionComponent.SetActive(false);


        //ShowLeaderBoard(); 
        DetermineWinner();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(waitAfterEnding);

        if(!perpetual)
        {
            PhotonNetwork.AutomaticallySyncScene = false; 
            PhotonNetwork.LeaveRoom(); 
            AdManager.Instance.ShowInterstitialAd(); 
        } 
        else 
        {
            if(PhotonNetwork.IsMasterClient)
            {
                NextMatchSend(); 
            }
        }
    }

    public void NextMatchSend()
    {
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NextMatch,
            null,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void NextMatchReceive()
    {
        state = GameState.Playing; 

        UIController.instance.endScreen.SetActive(false);
        UIController.instance.leaderboard.SetActive(false);

        foreach(PlayerInfo player in allPlayers)
        {
            player.kills = 0; 
            player.deaths = 0;
            player.money = 0; 
        }

        UpdateStatsDisplay();
        PlayerSpawner.instance.SpawnPlayer(); 
        SetUpTimer();
    }

    public void SetUpTimer() 
    {
        if(matchLength > 0)
        {
            currentMatchTime = matchLength;
            UpdateTimerDisplay();
        }
    }
    
    public void UpdateTimerDisplay() 
    {
        var timeToDisplay = System.TimeSpan.FromSeconds(currentMatchTime);
        UIController.instance.timerText.text = timeToDisplay.Minutes.ToString("00") + ":" + timeToDisplay.Seconds.ToString("00"); 
    }

    public void TimerSend()
    {
        object[] package = new object[] { (int)currentMatchTime, state };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.TimerSync,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void TimerReceive(object[] dataReceived)
    {
        currentMatchTime = (int)dataReceived[0]; 
        state = (GameState)dataReceived[1]; 

        UpdateTimerDisplay();

        UIController.instance.timerText.gameObject.SetActive(true);
    }

    public void WaitingTimerSend()
    {
        object[] package = new object[] { waitingTimer };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.WaitingTimerSync,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void WaitingTimerReceive(object[] dataReceived)
    {
        waitingTimer = (float)dataReceived[0];
        UIController.instance.waitingTimerText.text = "The Heist will begin in " + Mathf.CeilToInt(waitingTimer).ToString() + " seconds!";

        UIController.instance.leaderboardComponent.SetActive(false);
        UIController.instance.healthComponent.SetActive(false);
        UIController.instance.statsComponent.SetActive(false);
        UIController.instance.GunComponent.SetActive(false);
        UIController.instance.timeComponent.SetActive(false);
        UIController.instance.optionComponent.SetActive(false);




        if (waitingTimer <= 0)
        {
            UIController.instance.waitingScreen.SetActive(false);

            UIController.instance.leaderboardComponent.SetActive(true);
            UIController.instance.healthComponent.SetActive(true);
            UIController.instance.statsComponent.SetActive(true);
            UIController.instance.GunComponent.SetActive(true);
            UIController.instance.timeComponent.SetActive(true);
            UIController.instance.optionComponent.SetActive(true);

            state = GameState.Playing;


        }
    }

    public void DetermineWinner()
    {
        PlayerInfo winner = allPlayers[0];
        foreach (PlayerInfo player in allPlayers)
        {
            if (player.money > winner.money)
            {
                winner = player;
            }
        } 

        UIController.instance.winnerText.text = "Winner: " + winner.name + " with $" + winner.money;
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor;
    public int kills;
    public int deaths;
    public int money;
    public int rank; // New property

    public PlayerInfo(string _name, int _actor, int _kills, int _deaths, int _money)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
        money = _money;
        rank = 0; // Initialize rank to 0




    }
}