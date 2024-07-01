using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class PlayerSoundManager : MonoBehaviourPunCallbacks
{


    public static PlayerSoundManager instance;
    [Header("Gun SFX")]
    public AudioSource gunShootSource; 
    
    void Awake (){
        instance = this; 

        gunShootSource = GetComponent<AudioSource>(); 
    }


    [PunRPC]
    public void ShootSFX()
    {
        gunShootSource.Play(); 
    }

    
   
  
}
