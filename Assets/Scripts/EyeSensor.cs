using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeSensor : MonoBehaviour
{
    [Header("Cone Vision")]
    public int RayCount = 50;
    public float ConeRadius = 2f;
    public float ConeOffset = 3f;
    public float SphereCastRadius = 1f;
    [SerializeField] private LayerMask _ignoreMask;

    [Header("Spotlight Vision")]
    public int Segments = 10;
    public float FOV = 60f;
    public float ViewDistance = 10f;
    [HideInInspector] public Transform LastSeenPlayer {get; private set;} 
    private Mesh mesh;


    /// <summary>
    /// Draws a circle of physics sphere casts of radius ConeRadius, and
    /// ConeOffset units infront of the gameobject (i.e. a Cone). 
    /// </summary>
    /// <param name="origin"></param> Point to draw the cone from
    /// <param name="angle"></param> Degrees above or below the horizon aim the cone
    /// <returns></returns>
    public bool DetectPlayerInCone(Vector3 origin, float angle = 0)
    {
        for(int i = 0; i < RayCount; i++)
        {
            float r = ConeRadius * Mathf.Sqrt((float)i / RayCount);
            float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

            Vector3 targetDirection = Quaternion.Euler(angle, 0, 0) * new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), ConeOffset);
            
            // Gizmos
            Debug.DrawRay(origin, targetDirection, Color.red);
            // Physics raycasts 
            if (Physics.SphereCast(origin, SphereCastRadius, targetDirection, out RaycastHit hit, ~_ignoreMask))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    LastSeenPlayer = hit.transform;
                    return true;
                }
            }
        }
        return false;
    }

    // public Mesh DrawWedgeMesh()
    // {
    //     Mesh mesh = new Mesh();
        
    //     int[] triangles = new int[Segments * 4 + 2 + 2];
    //     Vector3[] vertices = new Vector3[2 * (Segments + 1 + 1)];
    //     Vector3 bottomLeft, bottomRight, topLeft, topRight;     
        
    //     // Left side
        


    //     float deltaAngle = FOV / Segments;
        
    //     for (int i = start; i < end; i++)
    //     {
    //         // Far side of each segment
    //         Vector3 dir = new(Mathf.Cos(theta), Mathf.Sin(phi), Mathf.Sin(theta));
    //         Physics.Raycast(transform.position, dir, out RaycastHit hitInfo, ViewDistance);
    //         vertices[i] = hitInfo.distance * transform.TransformDirection(dir);

    //         theta -= deltaAngle;
    //     }


        
    //     // Assign arrays
    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;

    //     return mesh;
    // }

    // private void OnValidate()
    // {
    //     mesh = DrawWedgeMesh();
    // }

    // private void OnDrawGizmos()
    // {
    //     if(mesh)
    //     {
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
    //     }
    // }
}
