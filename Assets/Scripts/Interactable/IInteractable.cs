using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    // Start is called before the first frame update
   
    float HoldDuration { get; }
    bool HoldInteract { get; }
    bool IsInteractable { get; }
    bool MultipleUse { get; }


    void OnInteract()
    {

        //print("Working");
    }
  
}
