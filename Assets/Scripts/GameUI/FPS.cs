using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using Photon.Pun; 

public class FPS : MonoBehaviourPunCallbacks
{
    public float fps;

    public TMPro.TextMeshProUGUI FPSCounter;



     void Start()
    {
        if(photonView.IsMine){
              InvokeRepeating("GetFPS", 1, 1);
        }
       
    }

    void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);

        FPSCounter.text = "FPS : " + fps.ToString(); 
    }
    
}
