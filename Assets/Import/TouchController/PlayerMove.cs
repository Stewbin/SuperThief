using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    
    public FixedJoystick joystick;
    public float SpeedMove = 5f;
    public CharacterController controller;

    private float Gravity = -9.81f; 
    public float GroundDistance = 0.3f;

    public float jumpHeight= 3f ; 
    public Transform Ground;

    public LayerMask layerMask;

    public bool isGrounded;

    public bool Pressed; 

    public Animator anim; 

    Vector3 velocity; 

    public GameObject playerModel; 

    public Transform modelGunPoint; 
    public Transform gunHolder; 

    public string nickname; 

    void Start()

    {
       
       if(photonView.IsMine){
        controller= GetComponent<CharacterController>();
        playerModel.SetActive(true);
       } else {
        gunHolder.parent = modelGunPoint;
        gunHolder.localPosition = Vector3.zero; 
        gunHolder.localRotation = Quaternion.identity; 
       }
       

        
       
        //Transform newTrans = SpawnManager.instance.GetSpawnPoints();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;
    }

    
    void Update()
    {

        if(photonView.IsMine) {


            isGrounded = Physics.CheckSphere(Ground.position, GroundDistance, layerMask);

        if(isGrounded && velocity.y < 0){
            velocity.y =-2f;
        }

        Vector3 Move = transform.right * joystick.Horizontal + transform.forward * joystick.Vertical;
        controller.Move(Move * SpeedMove * Time.deltaTime);

        if (isGrounded && Pressed){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Gravity);
            isGrounded = false;
        }

        velocity.y += Gravity * Time.deltaTime;
        controller.Move(velocity* Time.deltaTime);

        anim.SetBool("grounded", isGrounded);
        anim.SetFloat("speed", Move.magnitude);

        }

       
    }


       

        
    
}
