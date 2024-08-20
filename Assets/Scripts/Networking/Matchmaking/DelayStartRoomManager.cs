using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System; 
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class DelayStartRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int waitingRoomSceneIndex; 

   


    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this); 
    }

       public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this); 
    }

    public override void OnJoinedRoom()
    {
        //Called when we join a room 
        //load into waiting room scene
        
        SceneManager.LoadScene(waitingRoomSceneIndex); 
    }


    



}
