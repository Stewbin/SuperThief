using System.Collections;
using FiniteStateMachine;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class NotSeenPlayerDecision : Decision
{
    public float CoolDown = 5f;
    private float _coolDown = 0f;

    public override bool Decide(BaseStateMachine stateMachine)
    {
        EyeSensor eyeSensor = stateMachine.GetComponent<EyeSensor>();
        if(_coolDown == 0) 
            _coolDown += Time.deltaTime;
        if(_coolDown > CoolDown)
            _coolDown = 0;

        // If have not seen player within CoolDown secs, return true
        return !eyeSensor.SeePlayer() && _coolDown == CoolDown;
    }
}
