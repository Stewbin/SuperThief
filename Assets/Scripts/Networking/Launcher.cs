using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; 
using UnityEngine.UI; 

public class Launcher : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public static Launcher instance; 
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

    public List<RoomButton> allRoomButtons = new List<RoomButton>(); 
    
   


    void Awake(){
        instance = this; 
    }
    void Start()
    {
        CloseMenus(); 

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Network...";
        
       
        //Connect using settings we set up for photon netwrok 
        PhotonNetwork.ConnectUsingSettings(); 
    }

    void CloseMenus(){
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false); 
        roomScreen.SetActive(false); 
        errorScreen.SetActive(false); 
         roomBrowserScreen.SetActive(false); 
    }

    // Update is called once per frame

    public override void OnConnectedToMaster(){
        
    
    
    PhotonNetwork.JoinLobby(); 

    loadingText.text = "Joining Lobby..."; 

    }

    public override void OnJoinedLobby(){
    CloseMenus(); 
    menuButtons.SetActive(true);
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


    }

     public override void OnCreateRoomFailed(short returnCode, string message){

        errorText.text = "Failed to Create Room" + message;
        CloseMenus(); 
        errorScreen.SetActive(true); 


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

    public void QuitGame(){
        Application.Quit(); 
    }



    public void Test(){
        print("succesfully touch the input"); 
    }



    
}
