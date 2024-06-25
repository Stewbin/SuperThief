using UnityEngine;

namespace FiniteStateMachine
{
    public abstract class FSMAction : ScriptableObject
    {
        public abstract void Execute(global::BaseStateMachine stateMachine);
    }
}
