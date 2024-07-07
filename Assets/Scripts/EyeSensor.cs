using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSensor : MonoBehaviour
{
    [Header("Cone Vision")]
    public int RayCount = 50;
    public float ConeRadius = 2f;
    public float ConeOffset = 3f;
    [Header("Spotlight Vision")]
    public int vertices;
    [SerializeField] private LayerMask _ignoreMask;
    [HideInInspector] public Transform lastSeenPlayer {get; private set;} 

    /// <summary>
    /// Draws a circle of physics sphere casts of radius ConeRadius, and
    /// ConeOffset units infront of the gameobject (i.e. a Cone). 
    /// </summary>
    /// <returns>True if any of the raycasts hit a player, and false otherwise.</returns>    
    public bool DetectPlayerInCone()
    {
        for(int i = 0; i < RayCount; i++)
        {
            float r = ConeRadius * Mathf.Sqrt((float)i / RayCount);
            float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

            Vector3 targetDirection = transform.TransformDirection(new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), ConeOffset));
            
            // Gizmos
            Debug.DrawRay(transform.position, targetDirection, Color.red);
            // Physics raycasts 
            if (Physics.SphereCast(transform.position, 1, targetDirection, out RaycastHit hit, ~_ignoreMask))
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

}
