using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObjectToCamera : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        //
        mainCamera = Camera.main;

      
        if (mainCamera == null)
        {
           print("Main camera not found. Make sure the camera is tagged as 'MainCamera' in the scene.");
        }
    }

    void Update()
    {
       
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
        }
    }
}