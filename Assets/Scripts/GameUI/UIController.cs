using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

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
  
  //public TMP_Text gameMessageText; 



}
