using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System; 
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public static Launcher instance; 

    // Get Photon Friends
    public static Action GetPhotonFriends =  delegate{}; 
    public GameObject loadingScreen; 
    public TMP_Text loadingText; 
    public GameObject menuButtons; 
    
    public GameObject createRoomScreen; 
    public TMP_InputField roomNameInput; 

    public GameObject roomScreen; 
    public TMP_Text roomNameText; 

    public GameObject errorScreen;
    public TMP_Text errorText; 

    public GameObject roomBrowserScreen;
    public RoomButton theRoomButton; 

    public TMP_Text playerNameLabel; 
   private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public List<RoomButton> allRoomButtons = new List<RoomButton>(); 
    
   public GameObject nameInputScreen;
   public TMP_InputField nameInput;

   private static bool hasSetUsername; 

   public string levelToPlay;

   public GameObject startButton; 

   public GameObject roomTestButton; 

  
 

    void Awake(){
        instance = this; 
    }
    void Start()
    {
        CloseMenus(); 

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Server...";
        
       
        //Connect using settings we set up for photon netwrok 
    

        if(!PhotonNetwork.IsConnected){
                PhotonNetwork.ConnectUsingSettings();
        }

#if UNITY_EDITOR
    roomTestButton.SetActive(true);
#endif
       
    }

    void CloseMenus(){
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false); 
        roomScreen.SetActive(false); 
        errorScreen.SetActive(false); 
        roomBrowserScreen.SetActive(false); 
        nameInputScreen.SetActive(false);
    }

    public void OpenMenusButtonsFromCreateScreen(){
        menuButtons.SetActive(true);
        createRoomScreen.SetActive(false); 
    }

       public void OpenMenusButtonsFromSelectRoomPanel(){
        menuButtons.SetActive(true);
        roomScreen.SetActive(false); 
    }

       public void OpenMenusButtonsFromRoomBrowserScreen(){
        menuButtons.SetActive(true);
        roomBrowserScreen.SetActive(false); 
    }
       public void OpenMenusButtonsFromErrorScreen(){
        menuButtons.SetActive(true);
        errorScreen.SetActive(false); 
    }



    // Update is called once per frame

    public override void OnConnectedToMaster(){
        
    
    
    PhotonNetwork.JoinLobby(); 

    PhotonNetwork.AutomaticallySyncScene = true; 

    loadingText.text = "Joining Lobby..."; 

    }

    public override void OnJoinedLobby(){
    CloseMenus(); 
    menuButtons.SetActive(true);
    PhotonNetwork.NickName = UnityEngine.Random.Range(0,1000).ToString();
    print("You have succesfully connected to a Photon Lobby");

    GetPhotonFriends?.Invoke(); 


if(!hasSetUsername){
    CloseMenus();
    nameInputScreen.SetActive(true);

    if (PlayerPrefs.HasKey("playerName")){
        nameInput.text =PlayerPrefs.GetString("playerName");
    }
} else {
    PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
}


    }

   
    public void OpenRoomCreate(){
        CloseMenus();
        createRoomScreen.SetActive(true);
    }

    public void CreateRoom(){

        if(!string.IsNullOrEmpty(roomNameInput.text)){

            RoomOptions options = new RoomOptions(); 
            options.MaxPlayers = 8; 
            PhotonNetwork.CreateRoom(roomNameInput.text, options); 
            CloseMenus();
            loadingText.text = "Creating Room...";
            loadingScreen.SetActive(true); 
        }
    }

    public override void OnJoinedRoom(){

        CloseMenus();
        roomScreen.SetActive(true); 
        
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; 

        ListAllPlayers(); 

        if(PhotonNetwork.IsMasterClient){
            startButton.SetActive(true);
        } else {
            startButton.SetActive(false); 
        }
    }

     public void ListAllPlayers(){

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
//dd
        }   
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){

            TMP_Text newPlayerLabel = Instantiate(playerNameLabel,playerNameLabel.transform.parent);
            newPlayerLabel.text = newPlayer.NickName;
            newPlayerLabel.gameObject.SetActive(true); 

            allPlayerNames.Add(newPlayerLabel);
//
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){

ListAllPlayers();

    }



   
    public void CloseErrorScreen(){

        CloseMenus(); 
        menuButtons.SetActive(true);
    }

    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom(); 
        CloseMenus(); 
        loadingText.text = "Leaving the Room...";
        loadingScreen.SetActive(true); 
    }

    public override void OnLeftRoom(){
        
        CloseMenus(); 
        menuButtons.SetActive(true); 
    }


    public void OpenRoomBrowser(){
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }
    public void CloseRoomBrowser(){
        CloseMenus(); 
        roomBrowserScreen.SetActive(false);
        menuButtons.SetActive(true); 


    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
         
         foreach(RoomButton rb in allRoomButtons){

            Destroy(rb.gameObject);
         }

         allRoomButtons.Clear(); 

         theRoomButton.gameObject.SetActive(false); 

         for (int i  = 0; i < roomList.Count;  i++){

            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList){

                RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);

                newButton.SetButtonDetails(roomList[i]); 
                newButton.gameObject.SetActive(true ); 

                allRoomButtons.Add(newButton);


            }
        }
    }


    public void JoinRoom (RoomInfo inputInfo){

        PhotonNetwork.JoinRoom(inputInfo.Name); 

        CloseMenus(); 

        loadingText.text = "Joining Room";
        loadingScreen.SetActive(true);
    }

 
    public  void  SetNickname(){

        if(!string.IsNullOrEmpty(nameInput.text)){

           PhotonNetwork.NickName = nameInput.text; 

           PlayerPrefs.SetString("playerName", nameInput.text);
           CloseMenus();
           menuButtons.SetActive(true);

           hasSetUsername = true;
        }
    }



    public void StartGame(){

PhotonNetwork.LoadLevel(levelToPlay);
    }
 
    public override void OnMasterClientSwitched(Player newMasterClient){
         if(PhotonNetwork.IsMasterClient){
            startButton.SetActive(true);
        } else {
            startButton.SetActive(false); 
        }
    }


    public void QuickJoin(){
        RoomOptions options = new RoomOptions(); 
        options.MaxPlayers = 8; 
        PhotonNetwork.CreateRoom("Test");
        CloseMenus();
        loadingText.text = "Creating Room";
        loadingScreen.SetActive(true);
    }
    public void QuitGame(){
        Application.Quit(); 
    }



    public void Test(){
        print("succesfully touch the input"); 
    }

    //test playfab friend system 

    public void GoToPlayFabLogin()
    {
        SceneManager.LoadScene("LoginPlayFabTest");
    }

    public void GoToDelayMatchMaking()
    {
//hey
    }
   

  



    
}
