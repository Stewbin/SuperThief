using UnityEngine;

namespace FiniteStateMachine
{
    public class BaseState : ScriptableObject
    {
        public virtual void Execute(global::BaseStateMachine machine) { }
    }
}
