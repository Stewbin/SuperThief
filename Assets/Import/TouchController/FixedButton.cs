using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class FixedButton : MonoBehaviourPunCallbacks, IPointerUpHandler, IPointerDownHandler
{
    // Start is called before the first frame update
    [HideInInspector]
    public bool Pressed; 
    public void OnPointerDown(PointerEventData eventData){

        if(photonView.IsMine){
     Pressed = true;
        }
        
   

        

    }
     public void OnPointerUp(PointerEventData eventData){
      
      if(photonView.IsMine){
     Pressed = false;
        }
        
      
        
    }
}
