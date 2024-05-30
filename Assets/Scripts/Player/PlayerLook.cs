using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerLook : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera camera;
    [SerializeField] public float xRotation = 0f;
    [SerializeField] public float xSensitivty = 30f;
    [SerializeField] public float ySensivity = 30f;

    void Start()
    {
        //testing
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Look( Vector2 input )
    {
        float mouseX = input.x;
        float mouseY = input.y;
//calculate cam rotation
        xRotation -= (mouseY * Time.deltaTime) * ySensivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80);

        //rotate player 
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivty);

    }
}
