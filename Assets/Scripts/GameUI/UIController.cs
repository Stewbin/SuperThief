using System.Net.Mime;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Photon.Pun;

public class UIController : MonoBehaviour
{
  
  public static UIController instance; 

  private void Awake(){
    instance = this;
  }

 //public TMP_Text overheatedMessage; 
  //public Slider weaponTempSlider; 

  public Slider healthSlider;
  public GameObject deathScreen; 
  public TMP_Text deathText; 

  public TMP_Text killsText; 
  public TMP_Text deathsText; 

  public TMP_Text moneyText; 
  
  public GameObject leaderboard; 

  public GameObject endScreen;

  public TMP_Text waitingText; 
  public TMP_Text waitingTimerText; 

  public TMP_Text respawnText; 

  public TMP_Text currentAmmo; 
  public TMP_Text magazineSize; 
  
  public TMP_Text timerText; 

  
  public TMP_Text winnerText; 

  public TMP_Text localEliminationMessage;

  
 


  public GameObject waitingScreen;

   public GameObject healthComponent;
   public GameObject leaderboardComponent;
   public GameObject statsComponent;

    public GameObject timeComponent;

   public GameObject GunComponent;

  public Leaderboard leaderboardPlayerDisplay; 

  public TMP_Text currentHealthDisplay; 

  public TMP_Text currentMoneyAmount;  

  public TMP_Text killFeedText;

  public GameObject optionsScreen;
  
  public TMP_Text eliminationMessage;

  public float eliminationMessageDuration = 3f; 

  public GameObject optionComponent; 

  public TMP_Text moneyPopup; 

  
  public TMP_Text tipMessage;

  
  public TMP_Text debugMessage;

   [Header("Kill Feed")]
    public GameObject killFeedItemPrefab; // Ensure this is set in the Inspector
    public Transform killFeedContainer; // Ensure this is set in the Inspector
    public float killFeedDisplayDuration = 3f;

      public TMP_Text debugDeathMessage;

    public TMP_Text damageTextAmount; 

    [Header("Detect player")]

     public TMP_Text detectPlayerText;
     


  void Update()
  {
    
  }

  public void ShowHideOptions()
  {
    if(!optionsScreen)
    {
      optionsScreen.SetActive(true);
    } else {
      optionsScreen.SetActive(false);
    }
  }

  public void ReturnToMainMenus()
  {
    PhotonNetwork.AutomaticallySyncScene = false; 
    PhotonNetwork.LeaveRoom(); 


  }

  public void QuitGame()
  {
    Application.Quit();
  }

   public void OpenOptionScreen()
  {
    optionsScreen.SetActive(true);
  }

     public void CloseOptionScreen()
  {
    optionsScreen.SetActive(false);
  }

public void ShowEliminationMessage(string damager, string victim)
{
    // Display elimination message for all players
    debugDeathMessage.text = $"{damager} eliminated {victim}";
    StartCoroutine(ClearMessage(debugDeathMessage));
}

public void ShowLocalEliminationMessage(string victim)
{
    // Display local elimination message
    localEliminationMessage.text = $"You eliminated {victim}";
    StartCoroutine(ClearMessage(localEliminationMessage));
}

private IEnumerator ClearMessage(TMP_Text messageText)
{
    yield return new WaitForSeconds(3f); // Wait for 3 seconds
    messageText.text = ""; // Clear the message
}





}
