using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class ShootingRangeManager : MonoBehaviour
{
    

    public GameObject backButton; 
     public GameObject musicButton; 

     public AudioSource musicSource; 

     public bool isMusicOn; 

     private void Awake()
     {
        musicSource.Play(); 
        isMusicOn = true; 
     }

     public void ToggleMusic()
     {
        if(isMusicOn == true) 
        {
            musicSource.Stop();
            isMusicOn = false;  
        } else if (isMusicOn == false)
        {
         musicSource.Play();
        isMusicOn = true; 
        }
     }


     public void OpenMenu()
     {
        Launcher.instance.RefreshRoomList(); 
        SceneManager.LoadScene(1); 
     }

}
