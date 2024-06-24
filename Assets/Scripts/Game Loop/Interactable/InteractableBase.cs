using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VHS {
public class InteractableBase : MonoBehaviour, 
IInteractable
{
    
[Header("Interactable Settings")]
public float holdDuration;

[Space]
public bool holdInteract;

public bool multipleUse;

public bool isInteractable;

[SerializeField] private string tooltipMessage = "interact";

public float HoldDuration=> holdDuration; 
public bool HoldInteract => holdInteract; 
public bool MultipleUse => multipleUse; 
public bool IsInteractable => isInteractable; 

public string ToolTipMessage => tooltipMessage; 

public virtual void OnInteract(){
    //Debug.Log("INTERACTED:  " + gameObject.name); 
}


}
}
