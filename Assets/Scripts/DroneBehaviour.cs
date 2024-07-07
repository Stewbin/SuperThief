using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBehaviour : EnemyBehaviour
{
    [Header("Shooting & Aiming")]
    [SerializeField] private Transform _gunBase;
    [SerializeField] private AdvancedGunSystem _advancedGunSystem;
    
    [Header("Movement")]
    public float MoveSpeed;
    [Header("Path Finding")]
    private EyeSensor eyeSensor;
    public float DroneSize;
    public float MaxDistance;
    public float Range;
    public int RayCount = 20;
    

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        // Patrol

        //

        #region Movement VFX

        #endregion
    }


    #region Path Finding

    private Vector3 FindUnobstructedDirection()
    {
        List<Vector3> openDirections = new(RayCount);
        
        for (int i = 0; i < RayCount; i++)
        {
            float phi = Mathf.Acos(1 - 2 * ((float) i / RayCount)) / 2;
            float theta = Mathf.PI * ((1 - Mathf.Sqrt(5)) / 2) * i;

            // Convert from spherical to cartesian coordinates
            float z = Mathf.Cos(phi);
            float x = Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = Mathf.Sin(phi) * Mathf.Sin(theta);

            Ray ray = new(transform.position, new Vector3(x, y, z));

            // Shoot ray
            if(!Physics.SphereCast(ray, DroneSize / 2, MaxDistance))
            {
                openDirections.Add(ray.direction);
            }
        }

        float bestAlign = -1;
        Vector3 bestDir = transform.forward;
        foreach(var dir in openDirections)
        {
            float cosTheta = Vector3.Dot(dir, transform.forward);
            Debug.Assert(cosTheta <= 1);
            if(cosTheta > bestAlign)
            {
                bestDir = dir;
                bestAlign = cosTheta; 
            }
        }
        return bestDir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);
        if(Physics.Raycast(transform.position, transform.forward, 1f))
        {
            Gizmos.color = Color.red;
        }
    }

    #endregion

}