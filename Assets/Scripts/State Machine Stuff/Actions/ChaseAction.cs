using UnityEngine;
using FiniteStateMachine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "FSM/Actions/Chase")]
public class ChaseAction : FSMAction 
{
    [Header("Agent Movement")]
    public float MoveSpeed = 1f; 

    public override void Execute(BaseStateMachine stateMachine)
    {
        // Get components
        EyeSensor eyeSensor = stateMachine.GetComponent<EyeSensor>();
        NavMeshAgent agent = stateMachine.GetComponent<NavMeshAgent>();
        // Update agent stats
        agent.speed = MoveSpeed;

        if (!agent.pathPending) // Agent completed path or stuck
        {
            agent.SetDestination(eyeSensor.lastSeenPlayer.position);
        }
    }    
}