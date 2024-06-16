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

  public TMP_Text respawnText; 

  public TMP_Text currentAmmo; 
  public TMP_Text magazineSize; 
  
  public TMP_Text timerText; 
 


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

  
  //public TMP_Text gameMessageText; 


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


  



}
