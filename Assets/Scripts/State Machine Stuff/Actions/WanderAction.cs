using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

[CreateAssetMenu(menuName = "FSM/Actions/Wander")]
public class WanderAction : FSMAction
{
    public float WanderDistance = 3f;
    Vector3 HomePoint;
    
    public override void Execute(BaseStateMachine stateMachine)
    {
        // Initializations
        NavMeshAgent agent = stateMachine.GetComponent<NavMeshAgent>(); 
        agent.SetDestination(RandomPointFrom(HomePoint));
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