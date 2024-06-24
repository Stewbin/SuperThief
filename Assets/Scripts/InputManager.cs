using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private PlayerMovement motor;

    private PlayerLook look;

    public Joystick joyStick;



    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.onFoot;
        motor = GetComponent<PlayerMovement>();
        onFoot.Jump.performed += ctx => motor.Jump();
        look = GetComponent<PlayerLook>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //move using value from input action
        motor.Move(onFoot.Movement.ReadValue<Vector2>());

        
    }

    private void LateUpdate()
    {
        look.Look(onFoot.Look.ReadValue<Vector2>());

    }


    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
