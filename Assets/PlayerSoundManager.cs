using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class PlayerSoundManager : MonoBehaviourPunCallbacks
{


    public static PlayerSoundManager instance;
    public AudioSource footStepSource; 
    public AudioClip footStepSFX; 

    public AudioSource gunShootSource;
    public AudioClip[] allGunShootSFX;

     void awake()
    {

        instance = this; 
    }

    
    public void PlayFootStepSFX(){
       GetComponent<PhotonView>().RPC("PlayFootStepSFX_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void PlayFootStepSFX_RPC(){
        footStepSource.clip = footStepSFX; 
        footStepSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f); 
        footStepSource.volume = UnityEngine.Random.Range(0.2f, 0.35f); 

        footStepSource.Play(); 
    }

    public void PlayShootSFX(int index){
       GetComponent<PhotonView>().RPC("PlayShootSFX_RPC", RpcTarget.All, index);
    }

    
    [PunRPC]
    public void PlayShootSFX_RPC(int index){
        gunShootSource.clip = allGunShootSFX[index]; 
        gunShootSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f); 
        gunShootSource.volume = UnityEngine.Random.Range(0.2f, 0.35f); 

        gunShootSource.Play(); 
    }

   
  
}
