using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 

public class Menu : MonoBehaviour
{
  
    [SerializeField] public string instagramUrl; 

    [SerializeField] public TMP_Text playerUsernameDisplay; 

    [SerializeField] public string playerUsername; 

    public void Start() {

        playerUsername = PlayFabManager.PFC.userName; 
        playerUsernameDisplay.text = playerUsername;
        print("The player username is " + playerUsername);
    }
    public void Play(){
        //CLick Play to start playing
        SceneManager.LoadScene(2);
        print("Successfully started playing");
    }

    public void ShowAdScreen(){
        SceneManager.LoadScene(7);
        print("Successfully started Ad Scene");
    }


      public void ShowLoginScreen(){
        SceneManager.LoadScene("Authentication");
        print("Successfully in login Scene");
    }

    public void OpenInstagramUrl(){

        Application.OpenURL(instagramUrl); 
    }

    public void OpenSinglePlayerScene(){
        SceneManager.LoadScene(8);
    }

     public void LeaveSinglePlayer(){
        SceneManager.LoadScene(1);
    }

     public void DelayMatchMaking(){
        SceneManager.LoadScene(10);
    }

     public void OpenShop(){
        SceneManager.LoadScene("Construction");
    }



    
}
