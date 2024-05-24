
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public TextMeshProUGUI timerText;

    [SerializeField] public float elapsedTime; 
    
    [SerializeField] public float remainingTime;
  void  Update()
  {
    CountDown();
  }

  public void ElapsedTime(){
elapsedTime += Time.deltaTime; 
    int minutes = Mathf.FloorToInt(elapsedTime/ 60);
    int seconds = Mathf.FloorToInt(elapsedTime % 60);

    timerText.text = string.Format("Remaing Time : {0:00} : {1:00}", minutes , seconds); 
  }

   public void CountDown(){

    if (remainingTime > 0){
remainingTime -= Time.deltaTime; 

    } else if (remainingTime < 0){
        remainingTime = 0;

        
    }

    int minutes = Mathf.FloorToInt(remainingTime/ 60);
    int seconds = Mathf.FloorToInt(remainingTime % 60);

    timerText.text = string.Format("{0:00} : {1:00}", minutes , seconds); 
  }

  
}
