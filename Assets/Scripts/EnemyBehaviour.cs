using System.IO;
using UnityEngine;
using UnityEngine.AI;


public class EnemyBehaviour : MonoBehaviour
{
    [Header("Vision Parameters (in Degrees)")]
    public float xFOV = 60f;
    public float yFOV = 60f;
    [SerializeField] float Radius = 5f;
    [SerializeField] float RayDensity = 0.5f;
    [Header("Movement Parameters")]
    public float MoveSpeed = 1f;
    public float StoppingDistance = 0.5f;
    public float WanderDistance = 3f;
    [SerializeField] States CurrentState = States.Wander;
    private NavMeshAgent Agent;
    Vector3 HomePoint;
    public Transform Player;

    
    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        HomePoint = transform.position;      
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Agent.stoppingDistance = StoppingDistance;
        Agent.speed = MoveSpeed;       
        // Update CurrentState
        CurrentState = SensePlayer() ? States.Chase : States.Wander;
        
        if(CurrentState == States.Wander)
        {
            if(Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Agent.SetDestination(RandomPoint(HomePoint));
                transform.LookAt(Agent.velocity);
            }
            // Debug.Log($"Random point: {Agent.destination}");
        } 
        else if(CurrentState == States.Chase)
        {
            Agent.SetDestination(Player.position);
        }      
    }

    /// <summary>
    /// Draws a rectangular steradian of a sphere of Physics Raycasts.
    /// </summary>
    /// <returns>True if object tagged with "Player" is hit.</returns>
    bool SensePlayer()
    {
        // Convert degrees to radians
        float _xFOV = xFOV * (Mathf.PI / 180);
        float _yFOV = yFOV * (Mathf.PI / 180); 
        
        for(float theta = -_xFOV; theta < _xFOV; theta += _xFOV / RayDensity )
        {
            for(float phi = -_yFOV; phi < _yFOV; phi += _yFOV / RayDensity)
            {
                // Convert spherical to cartesian coordinates
                float y = Mathf.Sin(phi);
                float x = Mathf.Cos(phi) * Mathf.Sin(theta);
                float z = Mathf.Cos(phi) * Mathf.Cos(theta);

                // Package it as one vector, and convert it to world coordinates
                Vector3 Ray = transform.TransformDirection(new Vector3(x, y, z));
                Debug.DrawRay(transform.position, Ray * Radius, Color.red);
                
                RaycastHit hit;
                if(Physics.Raycast(transform.position, Ray, out hit, Radius, LayerMask.NameToLayer("Water")))
                    return hit.transform.CompareTag("Player");
                
            }
        }
        return false;
    }

    /// <summary>
    /// Returns a random point on the navmesh within a sphere of radius WanderDistance
    /// </summary>
    /// <param name="centerPoint"></param>
    /// <returns></returns>
    Vector3 RandomPoint(Vector3 centerPoint)
    {
        Vector3 randomPoint = centerPoint + (Random.insideUnitSphere * WanderDistance); // Random point in a sphere
        
        NavMeshHit hit;
        // Find closest point to randomPoint by projecting onto NavMesh 
        if (NavMesh.SamplePosition(randomPoint, out hit, WanderDistance, NavMesh.AllAreas))
            return hit.position; // Move to projected point
        else
            return centerPoint; // Return to center
    }
}

enum States
{
    Wander,
    Chase
}
