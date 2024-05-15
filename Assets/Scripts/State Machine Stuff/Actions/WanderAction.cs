using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

[CreateAssetMenu(menuName = "FSM/Actions/Wander")]
public class WanderAction : FSMAction
{
    
    public override void Execute(BaseStateMachine stateMachine)
    {
        // Initializations
        NavMeshAgent Agent = stateMachine.GetComponent<NavMeshAgent>(); // Use custom GetComponent() instead of the default
        Vector3 HomePoint = stateMachine.transform.position;
    }
}