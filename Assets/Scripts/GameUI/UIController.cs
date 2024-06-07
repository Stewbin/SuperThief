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

  public GameObject waitingScreen;

   public GameObject healthComponent;
   public GameObject leaderboardComponent;
   public GameObject statsComponent;

  public Leaderboard leaderboardPlayerDisplay; 

  public TMP_Text currentHealthDisplay; 
  
  //public TMP_Text gameMessageText; 



}
