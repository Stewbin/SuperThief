using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "FSM/Actions/Wander")]
public class WanderAction : FSMAction
{
    public float WanderDistance = 3f;
    public float MoveSpeed = 10f;
    public Vector3? homePoint = null;
    
    public override void Execute(BaseStateMachine stateMachine)
    {
        // Initializations
        NavMeshAgent agent = stateMachine.GetComponent<NavMeshAgent>(); 
        agent.speed = MoveSpeed;
        if(homePoint == null)
        {
            homePoint = stateMachine.transform.position;
        } 

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(RandomPointFrom((Vector3)homePoint));
            stateMachine.transform.LookAt(agent.velocity);
        }
        // Debug.Log($"Random point: {agent.destination}");
    }

    /// <summary>
    /// Returns a random point on the navmesh within a sphere of radius WanderDistance
    /// </summary>
    /// <param name="centerPoint"></param>
    /// <returns></returns>
    Vector3 RandomPointFrom(Vector3 centerPoint)
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