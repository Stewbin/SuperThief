using UnityEngine;

namespace FiniteStateMachine
{
    public class BaseStateMachineGraph : BaseStateMachine
    {
        [SerializeField] private FSMGraph _graph;
        public new BaseStateNode CurrentState { get; set; }
        
        public override void Init()
        {
            CurrentState = _graph.InitialState;
        }
        
        public override void Execute()
        {
            ((StateNode)CurrentState).Execute(this);
        }
    }
}