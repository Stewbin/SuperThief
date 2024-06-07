using UnityEngine;
using FiniteStateMachine;
using UnityEngine.AI;

public class ChaseAction : FSMAction 
{
    
    public override void Execute(BaseStateMachine stateMachine)
    {
        EyeSensor eyeSensor = stateMachine.GetComponent<EyeSensor>();
        NavMeshAgent agent = stateMachine.GetComponent<NavMeshAgent>();

        if (agent.pathPending)
        {
            agent.SetDestination(eyeSensor.GetLastSeenPlayer().position);
        }
    }    
}