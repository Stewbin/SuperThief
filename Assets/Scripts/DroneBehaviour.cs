using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBehaviour : EnemyBehaviour
{
    [Header("Shooting & Aiming")]
    [SerializeField] private Transform _gunBase;
    [SerializeField] private AdvancedGunSystem _advancedGunSystem;
    private Transform _targetPlayer;
    
    [Header("Movement")]
    public float MoveAcceleration;
    [Range(0, 90)] 
    public float MaxTilt;
    [Header("Path Finding")]
    private EyeSensor _eyeSensor;
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
        // Aim at target
        if(Aim(TargetPlayer))
        {
            _advancedGunSystem.Shoot();
        }

        // Movement
        transform.localPosition =  Vector3.forward;
    }

    public bool Aim(Transform target)
    {
        if(target.position.y < transform.position.y)
        {
            _gunBase.rotation = Quaternion.LookRotation(target.position, transform.up);
            return true;
        }
        return false;
    }

    public void Move(Vector3 destination)
    {
        // Move towards destination
        transform.position = Vector3.MoveTowards(transform.position, destination, MoveAcceleration * Time.time * Time.time);

        // Tilt in opposite direction of acceleration
        Quaternion maxTilt = Quaternion.Euler(0, -MaxTilt, 0);
        transform.rotation = Quaternion.Slerp(Quaternion.identity, maxTilt, MoveAcceleration * Time.time);
    }


    // #region Path Finding

    // private Vector3 FindUnobstructedDirection()
    // {
    //     List<Vector3> openDirections = new(RayCount);
        
    //     for (int i = 0; i < RayCount; i++)
    //     {
    //         float phi = Mathf.Acos(1 - 2 * ((float) i / RayCount)) / 2;
    //         float theta = Mathf.PI * ((1 - Mathf.Sqrt(5)) / 2) * i;

    //         // Convert from spherical to cartesian coordinates
    //         float z = Mathf.Cos(phi);
    //         float x = Mathf.Sin(phi) * Mathf.Cos(theta);
    //         float y = Mathf.Sin(phi) * Mathf.Sin(theta);

    //         Ray ray = new(transform.position, new Vector3(x, y, z));

    //         // Shoot ray
    //         if(!Physics.SphereCast(ray, DroneSize / 2, MaxDistance))
    //         {
    //             openDirections.Add(ray.direction);
    //         }
    //     }

    //     float bestAlign = -1;
    //     Vector3 bestDir = transform.forward;
    //     foreach(var dir in openDirections)
    //     {
    //         float cosTheta = Vector3.Dot(dir, transform.forward);
    //         Debug.Assert(cosTheta <= 1);
    //         if(cosTheta > bestAlign)
    //         {
    //             bestDir = dir;
    //             bestAlign = cosTheta; 
    //         }
    //     }
    //     return bestDir;
    // }

    // #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);
        if(Physics.Raycast(transform.position, transform.forward, 1f))
        {
            Gizmos.color = Color.red;
        }
    }

}