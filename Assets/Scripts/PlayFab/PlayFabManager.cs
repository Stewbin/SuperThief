using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; 
using System.Collections; 
//using PlayFab.PfEditor.Json;
using PlayFab.Json;
using JsonObject = PlayFab.Json.JsonObject;
using TMPro; 

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }

    public static PlayFabManager PFC; 


    [SerializeField] public string userName;
    [SerializeField] private string userEmail;
    [SerializeField] private string userPassword;

    [SerializeField] private string myID;

    [SerializeField] private int sceneIndex;

    

    public GameObject loginPanel;
    public GameObject addLoginPanel;
    public GameObject recoverButton;
    private const string EMAIL_KEY = "EMAIL";
    private const string PASSWORD_KEY = "PASSWORD";
    private const string USERNAME_KEY = "USERNAME";

    public TMP_Text errorTextMessage; 

    private void OnEnable()
    {
        if(PlayFabManager.PFC == null)
        {
            PlayFabManager.PFC = this;
        }else
        {
            if(PlayFabManager.PFC != null)
            {
             Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Awake()
    {
        // Implement proper singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "C3E0D";
        }

        //for testing purposes
        //PlayerPrefs.DeleteAll(); 
       //Auto Create An Accout for user
       AttemptAutoLogin();

       //get player currenciesnns
       GetVirtualCurrencies(); 
    
    }

    private void AttemptAutoLogin()
    {
        if (PlayerPrefs.HasKey(EMAIL_KEY) && PlayerPrefs.HasKey(PASSWORD_KEY))
        {
            userEmail = PlayerPrefs.GetString(EMAIL_KEY);
            userPassword = PlayerPrefs.GetString(PASSWORD_KEY);

            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }

    }

    public void PlayAsGuest()
    {

#if UNITY_ANDROID

    var requestAndroid = new LoginWithAndroidDeviceIDRequest {AndroidDeviceId = ReturnMobileID(), CreateAccount = true};
    PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif

#if UNITY_IOS

    var requestIOS =  new LoginWithIOSDeviceIDRequest {DeviceId = ReturnMobileID(), CreateAccount = true};
    PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);

#endif
    
    }
        


#region Authentication

    private bool IsValidUsername()
    {
        return userName.Length >= 3 && userName.Length <= 24;
    }

    public void GetUsername(string username)
    {
        userName = username;
        PlayerPrefs.SetString(USERNAME_KEY, userName);
    }

   

    private void UpdateDisplayName(string displayName)
    {

        userName = displayName; 
        print($"Updating display name to {displayName}");

        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = userName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnLoginMobileFailure);
    }


    private void OnFailure(PlayFabError error)
    {
       print($"PlayFab error: {error.GenerateErrorReport()}");
    }

    private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        print("Display name updated successfully");
    }

    public void GetUserEmail(string email)
    {
        userEmail = email;
    }

    public void GetUserPassword(string password)
    {
        userPassword = password;
    }

    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        print("Successfully registered");
        SaveCredentials();
        loginPanel.SetActive(false);
         //Get All Player Statistics
        GetStatistics(); 
        UpdateDisplayName(userName); 
        myID =result.PlayFabId;

        GetPlayerData(); 

        //open game menu
        LaunchGameMenu();

        //Get Virtual Currency
        GetVirtualCurrencies();
    }
    
    public void OnLoginSuccess(LoginResult result)
    {
        print("Successfully logged in");
        SaveCredentials();
        loginPanel.SetActive(false);
        recoverButton.SetActive(false);
        //Get All Player Statistics
        GetStatistics(); 

        myID =result.PlayFabId;
        GetPlayerData(); 

        //open game menu
        LaunchGameMenu();

        //Get Virtual Currency
        GetVirtualCurrencies();

        // try to display inverntory
        
    }

    public void OnLoginMobileSuccess(LoginResult result)
    {
        print("Successfully logged in");
      //Get All Player Statistics
        GetStatistics(); 
        myID =result.PlayFabId;
        loginPanel.SetActive(false);
         GetPlayerData(); 

         //open game menu
       LaunchGameMenu();


       //Get Virtual Currency
        GetVirtualCurrencies();
    }

    private void SaveCredentials()
    {
        PlayerPrefs.SetString(EMAIL_KEY, userEmail);
        PlayerPrefs.SetString(PASSWORD_KEY, userPassword);
        //myID =result.PlayFabId;
    }

    public void OnLoginFailure(PlayFabError error)
    {
        print($"Login failed: {error.ErrorMessage}");
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);

        //display error text 
        errorTextMessage.text = error.ErrorMessage;
      
    }

    public void OnLoginMobileFailure(PlayFabError error)
    {
        print($"Login failed: {error.ErrorMessage}");
       
      
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest 
        { 
            Email = userEmail, 
            Password = userPassword, 
            Username = userName 
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    public void OnRegisterFailure(PlayFabError error)
    {
        print($"Registration failed: {error.ErrorMessage}");

    }

    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID; 
    }

    public void OpenAddLogin()
    {
        addLoginPanel.SetActive(true); 
    }

      public void OnClickAddLogin()
    {
        //print($"Login failed: {error.ErrorMessage}");
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, OnRegisterFailure);
    }

    public void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {
        print("Successfully registered");
        SaveCredentials();
        addLoginPanel.SetActive(false); 
         //Get All Player Statistics
        GetStatistics(); 
         GetPlayerData(); 
         //myID = result.PlayFabId;

         //open game menu
        LaunchGameMenu();

        //Get Virtual Currency
        GetVirtualCurrencies();
    }

    public void LaunchGameMenu()
    {
        SceneManager.LoadScene(sceneIndex); 
    }
    
    #endregion Authentication
 
public int playerLevel;
public int gameLevel;
public int playerHealth;
public int playerDamage;
public int playerKills;
public int playerDeaths;
public int gamePlayed;

public int dailyLogin;
  
public int playerThiefCoins;
public int playerHeistDiamonds;

#region Player Statistics
public void SetStatistics()
{
    //Update Player Statistics
    PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
    {
        // request.Statistics is a list, so multiple statistics objects can be defined if required. 
        Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate { StatisticName = "PlayerLevel", Value = playerLevel },
            new StatisticUpdate { StatisticName = "GameLevel", Value = gameLevel },
            new StatisticUpdate { StatisticName = "PlayerThiefCoins", Value = playerThiefCoins },
            new StatisticUpdate { StatisticName = "PlayerHeistDiamonds", Value = playerHeistDiamonds },
            
        }
    },
    result => { print("User Statistics Updated"); },
    error => { print(error.ErrorMessage); }
    );
}
public void GetStatistics()
{
    //Get Player Statistics
    PlayFabClientAPI.GetPlayerStatistics(
        new GetPlayerStatisticsRequest(), 
        OnGetStatistics, 
        error => { print(error.ErrorMessage); }
    );
}

public void OnGetStatistics(GetPlayerStatisticsResult result)
{
    print("Received the following statistics");
    foreach(var eachStat in result.Statistics)
    {
        print("Statistics(" + eachStat.StatisticName + ") :" + eachStat.Value);

        switch(eachStat.StatisticName)
        {
            case "PlayerLevel":
                playerLevel = eachStat.Value;
                break;
            case "GameLevel":
                gameLevel = eachStat.Value;
                break;
            case "PlayerThiefCoins":
                playerThiefCoins = eachStat.Value;
                break;
            case "PlayerHeistDiamonds":
                playerHeistDiamonds = eachStat.Value;
                break;
        }
    }
}
public void StartCloudUpdatePlayerStats()
{
   PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    {
        FunctionName = "UpdatePlayerStats",
        FunctionParameter = new { Level = playerLevel, ThiefCoins = playerThiefCoins, HeistDiamonds = playerHeistDiamonds },
        GeneratePlayStreamEvent = true
    }, OnCloudUpdateStats, OnErrorShared);
}

private static void OnCloudUpdateStats(ExecuteCloudScriptResult result)
{
    print(PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer));
    JsonObject jsonResult = (JsonObject)result.FunctionResult; 

    object messageValue; 
    jsonResult.TryGetValue("messageValue", out messageValue);

}

private static void OnErrorShared(PlayFabError error)
{
    print(error.ErrorMessage); 
}
#endregion Player Stats

public GameObject leaderboardPanel; 
public GameObject listingPrefab; 
public Transform listingContainer; 


#region Leaderboard



public void GetLeaderboard()
{

var requestLeaderboard = new GetLeaderboardRequest {StartPosition = 0, StatisticName = "PlayerLevel", MaxResultsCount = 100};

PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderBoard, OnErrorLeaderBoard);

}
  void OnGetLeaderBoard(GetLeaderboardResult result)
{
    //print(result.Leaderboard[0].StatValue);

    leaderboardPanel.SetActive(true);
    foreach(PlayerLeaderboardEntry player in result.Leaderboard)
    {
        GameObject tempListing = Instantiate(listingPrefab, listingContainer);

        LeaderboardListing leaderboardListing = tempListing.GetComponent<LeaderboardListing>();
        leaderboardListing.playerNameText.text = player.DisplayName; 
        leaderboardListing.playerScoreText.text = player.StatValue.ToString(); 

        print(player.DisplayName + ":" + player.StatValue);
    }
   
}

 public void CloseLeaderboardPanel()
{
    leaderboardPanel.SetActive(false);

    for(int i = listingContainer.childCount -1 ; i >= 0; i--)
    {
        Destroy(listingContainer.GetChild(i).gameObject);
    }

}

  void OnErrorLeaderBoard(PlayFabError error)
{
    print(error.ErrorMessage);

}
#endregion Leaderboard

#region Player Data

public void GetPlayerData()
{
    PlayFabClientAPI.GetUserData(new GetUserDataRequest()
    {
        PlayFabId = myID,
        Keys = null
    }, UserDataSuccess, UserDataError);
}

public void UserDataSuccess(GetUserDataResult result)
{
    if (result.Data == null || !result.Data.ContainsKey("Skins"))
    {
        print("Skins not set");
    }
    else 
    {
        PersistentData.PD.SkinsStringToData(result.Data["Skins"].Value);
    }
}

public void SetUserData(string SkinsData)
{
    PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
    {
        Data = new Dictionary<string, string>()
        {
            {"Skins", SkinsData}
        }
    }, SetDataSuccess, SetDataFailure);
}

public void SetDataSuccess(UpdateUserDataResult result)
{
    print(result.DataVersion);  
}

public void UserDataError(PlayFabError error)
{
    print(error.ErrorMessage);
}

public void SetDataFailure(PlayFabError error)
{
    print(error.ErrorMessage);
}

#endregion Player Data

#region Friends System

public Transform friendScrollView;
List<FriendInfo> myFriends;

void DisplayFriends(List<FriendInfo> friendsCache)
{
    if (friendsCache == null || friendsCache.Count == 0)
    {
        Debug.Log("No friends to display.");
        return;
    }

    foreach (FriendInfo friend in friendsCache)
    {
        bool isFound = false;
        
        if (!isFound)
        {
            GameObject listing = Instantiate(listingPrefab, friendScrollView);
            LeaderboardListing tempListing = listing.GetComponent<LeaderboardListing>();
            
            if (tempListing != null && tempListing.playerNameText != null)
            {
                tempListing.playerNameText.text = friend.TitleDisplayName;
                Debug.Log($"Added friend: {friend.TitleDisplayName}");
            }
            else
            {
                Debug.LogError("LeaderboardListing component or playerNameText is null.");
            }
        }
    }

    myFriends = friendsCache;
}

List<FriendInfo> _friends = null;

public void GetFriends()
{
    PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
    {
        //IncludeSteamFriends = false,
        //IncludeFacebookFriends = false
    }, result => {
        _friends = result.Friends;
        DisplayFriends(_friends);
    }, DisplayPlayFabError);
}

public void DisplayPlayFabError(PlayFabError error)
{
    print(error.ErrorMessage);
}

enum FriendIdType { PlayFabId, Username, Email, DisplayName };

void AddFriend(FriendIdType idType, string friendId)
{
    var request = new AddFriendRequest();
    switch (idType)
    {
        case FriendIdType.PlayFabId:
            request.FriendPlayFabId = friendId;
            break;
        case FriendIdType.Username:
            request.FriendUsername = friendId;
            break;
        case FriendIdType.Email:
            request.FriendEmail = friendId;
            break;
        case FriendIdType.DisplayName:
            request.FriendTitleDisplayName = friendId;
            break;
    }
    // Execute request and update friends when we are done
    PlayFabClientAPI.AddFriend(request, result => {
        Debug.Log("Friend added successfully!");
    }, DisplayPlayFabError);
}

string friendSearch;
[SerializeField] GameObject friendPanel;

public void InputFriendID(string id)
{
    friendSearch = id;
}

public void SubmitFriendRequest()
{
    AddFriend(FriendIdType.Username, friendSearch);
}

public void OpenCloseFriends()
{
    friendPanel.SetActive(!friendPanel.activeInHierarchy);
}

IEnumerator WaitForFriend()
{

yield return new WaitForSeconds(2f);
GetFriends(); 

}

public void RunWaitFunction()
{
    StartCoroutine(WaitForFriend());
}

#endregion Friends System


#region Daily Rewards
#endregion Daily Rewards

#region Currency System
public const string COINS_KEY = "Coins";
public const string DIAMONDS_KEY = "Diamonds";

[Header("Coins UI")]

public TMP_Text ThiefCoinsValuesText; 

public TMP_Text HeistDiamondsValuesText; 


public void GetVirtualCurrencies()
{
    PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnGetUserInventoryError);
}


public void OnGetUserInventorySuccess(GetUserInventoryResult result)
{
    //Thief Coins
    int coins = result.VirtualCurrency["TC"];
    ThiefCoinsValuesText.text = coins.ToString(); 

      //Heist Diamonds
    int diamonds = result.VirtualCurrency["HD"];
    HeistDiamondsValuesText.text = diamonds.ToString(); 

    print("You currently have" + diamonds);

    //ThiefCoinsValuesText.text = "T"


    print("You currently have" + coins);


}

public void OnGetUserInventoryError(PlayFabError error)
{
    print(error.ErrorMessage);
}


//public void GrantVirtualCurrency()
//{
  
//}
#endregion Currency System

}