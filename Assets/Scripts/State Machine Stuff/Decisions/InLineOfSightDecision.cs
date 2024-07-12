using System.Collections;
using System.Collections.Generic;
using FiniteStateMachine;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decision/In Line Of Sight")]
public class InLineOfSightDecision : Decision
{
    public override bool Decide(BaseStateMachine stateMachine)
    {
        EyeSensor eyeSensor = stateMachine.GetComponent<EyeSensor>();
        return eyeSensor.DetectPlayerInCone();
    }

}
