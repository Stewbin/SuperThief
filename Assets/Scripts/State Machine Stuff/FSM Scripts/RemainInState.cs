using UnityEngine;

namespace FiniteStateMachine
{
    [CreateAssetMenu(menuName = "FSM/Remain In State", fileName = "RemainInState")]
    public sealed class RemainInState : BaseState
    {
      // Intentionally blank,
      // in case a Transition wants to evaluate to "Don't do anything".
    }
}
