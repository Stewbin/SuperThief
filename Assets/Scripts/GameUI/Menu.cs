using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Menu : MonoBehaviour
{
  
    public void Play(){
        //CLick Play to start playing
        SceneManager.LoadScene(1);
        print("Successfully started playing");
    }

    public void ShowAdScreen(){
        SceneManager.LoadScene(6);
        print("Successfully started Ad Scene");
    }


      public void ShowLoginScreen(){
        SceneManager.LoadScene(0);
        print("Successfully in login Scene");
    }
    
}
