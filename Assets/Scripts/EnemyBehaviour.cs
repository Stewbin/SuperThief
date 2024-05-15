using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Vision Parameters (in Degrees)")]
    public float xFOV = 60f;
    public float yFOV = 60f;
    [SerializeField] float Radius = 5f;
    [SerializeField] float TurnFraction = 2f;
    [Header("Movement Parameters")]
    public float MoveSpeed = 1f;
    public float StoppingDistance = 0.5f;

    
    // Start is called before the first frame update
    void Start()
    {
        var NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.stoppingDistance = StoppingDistance;
        NavMeshAgent.speed = MoveSpeed;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        DrawVisionCone();
        //Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(Mathf.PI / 3),0,Mathf.Sin(Mathf.PI / 3)) * Radius, Color.green);
    }

    void DrawVisionCone()
    {
        // Convert degrees to radians
        float _xFOV = xFOV * (Mathf.PI / 180);
        float _yFOV = yFOV * (Mathf.PI / 180); 
        
        for(float theta = -_xFOV; theta < xFOV; theta += 1)//xFOV / TurnFraction )
        {
            for(float phi = -_yFOV; phi < yFOV; phi += 1)//yFOV / TurnFraction)
            {
                // Convert spherical to cartesian coordinates
                float y = Mathf.Sin(phi);
                float x = Mathf.Cos(phi) * Mathf.Sin(theta);
                float z = Mathf.Cos(phi) * Mathf.Cos(theta);

                Vector3 Ray = new Vector3(x, y, z) * Radius;
                Debug.DrawRay(transform.position, Ray, Color.red);
            }
        }
    }
}


