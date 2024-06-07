using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class CameraLook : MonoBehaviourPunCallbacks
{
    private float XMove;
    private float YMove;
    private float XRotation;
    [SerializeField] private Transform PlayerBody;
    public Vector2 LockAxis;
    [SerializeField] public float Sensivity = 5f;


    
    void LateUpdate()
    {
       
  
        XMove = LockAxis.x * Sensivity * Time.deltaTime;
        YMove = LockAxis.y * Sensivity * Time.deltaTime;
        XRotation -= YMove;
        XRotation = Mathf.Clamp(XRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(XRotation,0,0);
        PlayerBody.Rotate(Vector3.up * XMove);

       
       
    }
       
    
}
