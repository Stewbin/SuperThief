using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UIElements;

public class EyeSensor : MonoBehaviour
{
    public int RayCount = 50;
    public float ConeRadius = 2f;
    public float ConeOffset = 3f;
    [SerializeField] private LayerMask _ignoreMask;
    private Transform lastSeenPlayer; // DON'T link in inspector! 

    /// <summary>
    /// Draws a circle of physics raycasts of radius ConeRadius, and
    /// ConeOffset units infront of the gameobject (i.e. a Cone). 
    /// </summary>
    /// <returns>True if any of the raycasts hit a player, and false otherwise.</returns>    
    public bool SeePlayer()
    {
        for(int i = 0; i < RayCount; i++)
        {
            float r = ConeRadius * Mathf.Sqrt((float)i / RayCount);
            float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

            Vector3 ray = transform.TransformDirection(new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), ConeOffset));
            
            // Gizmos
            Debug.DrawRay(transform.position, ray, Color.red);
            // Physics raycasts
            RaycastHit hit;
            if (Physics.Raycast(transform.position, ray, out hit, ~_ignoreMask))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    lastSeenPlayer = hit.transform;
                    return true;
                }
            }
        }
        return false;
    }

    public Transform GetLastSeenPlayer()
    {
        return lastSeenPlayer;
    }
}
