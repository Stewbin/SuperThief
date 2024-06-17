using UnityEngine;

namespace FiniteStateMachine
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(BaseStateMachine state);
    }
}
