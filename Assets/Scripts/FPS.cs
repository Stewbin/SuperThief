using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class FPS : MonoBehaviour
{
    public float fps;

    public TMPro.TextMeshProUGUI FPSCounter;



     void Start()
    {
        InvokeRepeating("GetFPS", 1, 1);
    }

    void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);

        FPSCounter.text = "FPS : " + fps.ToString(); 
    }
    
}
