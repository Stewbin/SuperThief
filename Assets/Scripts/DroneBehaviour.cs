using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayFab.EconomyModels;
using UnityEngine;

public class DroneBehaviour : EnemyBehaviour
{
    [Header("Shooting & Aiming")]
    [SerializeField] private Transform _gunBase;
    [SerializeField] private AdvancedGunSystem _advancedGunSystem;
    private EyeSensor _eyeSensor;
    [Range(0, 90)] public float SensorAngle;
    [SerializeField] private readonly float aggroTime;
    private float _aggroTime;
    
    [Header("Movement")]
    public Vector3 currentDestination;
    public float MinHeight = 15f;
    public float MoveAcceleration;
    [Range(0, 90)] public float MaxTilt;

    [Header("Path Finding")]
    public float DroneWidth;
    public float MaxDistance;
    public float Range;
    public int RayCount = 20;
    

    // Start is called before the first frame update
    void Start()
    {
         _eyeSensor = GetComponent<EyeSensor>();
        //  _advancedGunSystem = GetComponent<AdvancedGunSystem>();
        currentDestination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nextDestination = currentDestination;
        bool seenPlayer = _eyeSensor.DetectPlayerInCone(_gunBase.position, SensorAngle);
        if(CurrentState == State.Searching)
        {
            // Exit Searching state
            if(seenPlayer)
            {
                TargetPlayer = _eyeSensor.LastSeenPlayer;
                CurrentState = State.Hunting;
            }

            // Move to random point
            Vector3 randomPoint = transform.position + (Random.insideUnitSphere * 10f); // Random point in a sphere            
            nextDestination = randomPoint;

            // Reset aggro meter
            _aggroTime = aggroTime;
        }
        else if(CurrentState == State.Hunting)
        {
            // Aim at target
            if(Aim(TargetPlayer))
                {_advancedGunSystem.Shoot();}
    
            // Exit Hunting state
            if(!seenPlayer)
                {_aggroTime -= Time.deltaTime;}
            if(aggroTime <= 0) 
                {CurrentState = State.Searching;}

            // Move to player
            nextDestination = TargetPlayer.position;
        }

        // Drone must be above certain height at all times
        if(currentDestination.y < MinHeight)
        {
            currentDestination += transform.up * (MinHeight - transform.position.y);
        }

        // Move to destination
        if(MoveTo(currentDestination))
        {
            // Update to next destination if arrived at current one
            currentDestination = nextDestination;
            Debug.Log("Destination arrived.");
        }

        // Testing
        Debug.Log($"State is {CurrentState}");

        // Avoid obstacles
        
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

    public bool MoveTo(Vector3 destination)
    {
        float vel = MoveAcceleration * Time.time;
        // Move towards destination
        transform.position = Vector3.MoveTowards(transform.position, destination, vel * Time.deltaTime);

        // Tilt in opposite direction of acceleration
        Quaternion maxTilt = Quaternion.Euler(-MaxTilt, 0, 0);
        transform.rotation = Quaternion.Slerp(Quaternion.identity, maxTilt, vel * Time.deltaTime);

        // Return if arrived
        return Vector3.Distance(transform.position, destination) < 0.01f;
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
            if(!Physics.SphereCast(ray, DroneWidth / 2, MaxDistance))
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

    #endregion
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