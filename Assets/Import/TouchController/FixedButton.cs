using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class FixedButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    // Start is called before the first frame update
    [HideInInspector]
    public bool Pressed; 
    public void OnPointerDown(PointerEventData eventData){

     Pressed = true;
    
    }
     public void OnPointerUp(PointerEventData eventData){
       
     Pressed = false;
                     
    }
}
