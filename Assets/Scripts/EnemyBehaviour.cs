using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Vision Parameters (in Degrees)")]
    public float xFOV = 60f;
    public float yFOV = 60f;
    [SerializeField] float Radius = 5f;
    [SerializeField] float TurnFraction = 0.5f;
    [Header("Movement Parameters")]
    public float MoveSpeed = 1f;
    public float StoppingDistance = 0.5f;
    public float WanderDistance = 1f;
    States CurrentState = States.Wander;
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
    void Update()
    {
        Agent.stoppingDistance = StoppingDistance;
        Agent.speed = MoveSpeed;       
        // Update CurrentState
        CurrentState = SensePlayer() ? States.Chase : States.Wander;

        if(CurrentState == States.Wander)
        {
            if(Agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                Agent.SetDestination(RandomPoint(HomePoint));
            }
        } 
        if(CurrentState == States.Chase)
        {
            Agent.SetDestination(Player.position);
        }
    }

    bool SensePlayer()
    {
        // Convert degrees to radians
        float _xFOV = xFOV * (Mathf.PI / 180);
        float _yFOV = yFOV * (Mathf.PI / 180); 
        
        for(float theta = -_xFOV; theta < _xFOV; theta += _xFOV / TurnFraction )
        {
            for(float phi = -_yFOV; phi < _yFOV; phi += _yFOV / TurnFraction)
            {
                // Convert spherical to cartesian coordinates
                float y = Mathf.Sin(phi);
                float x = Mathf.Cos(phi) * Mathf.Sin(theta);
                float z = Mathf.Cos(phi) * Mathf.Cos(theta);

                Vector3 Ray = new Vector3(x, y, z);
                RaycastHit hit;
                if(Physics.Raycast(transform.position, Ray, out hit, Radius))
                    return hit.transform.CompareTag("Player");
            }
        }
        return false;
    }

    Vector3 RandomPoint(Vector3 centerPoint)
    {
        Vector3 randomPoint = centerPoint + Random.insideUnitSphere * WanderDistance; //random point in a sphere
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) 
            return hit.position;
        else 
            return Vector3.zero;
    }
}

enum States
{
    Wander,
    Chase
}
