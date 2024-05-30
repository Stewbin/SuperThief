using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Menu : MonoBehaviour
{
  
    public void Play(){
        //CLick Play to start playing
        SceneManager.LoadScene("Arena");
        print("Successfully started playing");
    }
    
}
