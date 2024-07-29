using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 

public class Menu : MonoBehaviour
{
  

    public static Menu instance; 
    [SerializeField] public string instagramUrl; 
    [SerializeField] public string discordUrl; 
    [SerializeField] public string privacyUrl; 

    public TMP_Text playerUsernameDisplay; 

     public TMP_Text HeistDiamondsValuesText; 
    [SerializeField] public string playerUsername; 

    

    [Header("UI Panels")]

    public GameObject shopPanel; 

    public GameObject lobbyMusic; 

    [SerializeField] public GameObject settingsPanel;
    
    [SerializeField] public GameObject deletionPanel;

     [SerializeField] public GameObject matchMaking; 


    private void Start()
    {
        // Show banner ad when main menu loads
        AdManager.Instance.ShowBannerAd();
    }

    private void OnDisable()
    {
        // Hide banner ad when leaving main menu
        AdManager.Instance.HideBannerAd();
    }
    public void Awake() {

        instance = this; 


        matchMaking.SetActive(false); 

        playerUsername = PlayerPrefs.GetString("USERNAME");
        HeistDiamondsValuesText.text = PlayerPrefs.GetInt("Diamonds").ToString(); 
        playerUsernameDisplay.text = playerUsername;
        print("The player username is " + playerUsername);

        shopPanel.SetActive(false); 
        settingsPanel.SetActive(false); 

        lobbyMusic.SetActive(true);

        AdManager.Instance.ShowBannerAd(); 
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

     public void OpenDiscordUrl(){
        Application.OpenURL(discordUrl); 
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true); 
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false); 
    }

     public void CloseDeletionPanel()
    {
        deletionPanel.SetActive(false); 
        settingsPanel.SetActive(false); 
    }

    public void OpenPrivacy()
    {
       Application.OpenURL(privacyUrl); 
    }

     public void OnClickDeleteAccount()
    {
        deletionPanel.SetActive(true); 
    }

 




    public void OpenSinglePlayerScene(){
        SceneManager.LoadScene(18);
    }

     public void LeaveSinglePlayer(){
        SceneManager.LoadScene(1);
    }

     public void DelayMatchMaking(){
        DelayStartLobbyManager.instance.DelayStart(); 
        matchMaking.SetActive(true);
    }

    public void OpenShop(){
        shopPanel.SetActive(true); 
    }

    public void CloseShop(){
        shopPanel.SetActive(false); 
    }


    public void CustomRoom()
    {
        SceneManager.LoadScene("Custom");
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

public void PlayStopMusic()
{
    bool newState = !lobbyMusic.activeSelf;
    lobbyMusic.SetActive(newState);
    
    if (newState)
    {
        print("Lobby music started");
    }
    else
    {
        print("Lobby music stopped");
    }
}
 


    
}
