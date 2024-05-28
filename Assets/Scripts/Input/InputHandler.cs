using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace VHS
{
    public class InputHandler : MonoBehaviour
    {

        [SerializeField] public Vector2 touchPosition; 

        [SerializeField] public InteractionInputData interactionInputData;
        void Start()
        {
            interactionInputData.Reset();

        }


        void Update()
        {
            GetInteractionInputData();
            TouchInputMobile(); 
        }

      void GetInteractionInputData()
{
    if (Touchscreen.current.primaryTouch.press.isPressed)
    {
        interactionInputData.InteractedClicked = true;
        //print("Touch press detected");
    }
    else if (!Touchscreen.current.primaryTouch.press.isPressed)
    {
        interactionInputData.InteractedReleased = true;
        //print("Touch release detected");
    }
    else
    {
        interactionInputData.InteractedClicked = false;
        interactionInputData.InteractedReleased = false;
    }
}

        void TouchInputMobile()
        {

            if (!Touchscreen.current.primaryTouch.press.isPressed)
            {

                return; 
            }

            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            //print(touchPosition);



        }

    }


    
}
