using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TouchController : MonoBehaviourPunCallbacks
{
    public FixedTouchField _FixedTouchField;
    public CameraLook _CameraLook;

   // public PlayerMove _PlayerMove;
   // public FixedButton _FixedButton;

   

    
    void Update()
    {

      
_CameraLook.LockAxis = _FixedTouchField.TouchDist;
     

    }
}
