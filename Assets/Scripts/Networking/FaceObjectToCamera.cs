using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObjectToCamera : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        
    }

    void Update()
    {
       
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
        }
    }
}