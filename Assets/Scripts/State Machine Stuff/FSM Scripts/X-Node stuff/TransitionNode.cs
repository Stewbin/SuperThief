namespace FiniteStateMachine
{
    [CreateNodeMenu("Transition")]
    public sealed class TransitionNode : FSMNode
    {
        public Decision Decision;
        [Output] public BaseStateNode TrueState;
        [Output] public BaseStateNode FalseState;
        public void Execute(BaseStateMachineGraph stateMachine)
        {
            var trueState = GetFirst<BaseStateNode>(nameof(TrueState));
            var falseState = GetFirst<BaseStateNode>(nameof(FalseState));
            var decision = Decision.Decide(stateMachine);
            if (decision && !(trueState is RemainInStateNode))
            {
                stateMachine.CurrentState = trueState;
            }
            else if(!decision && !(falseState is RemainInStateNode))
                stateMachine.CurrentState = falseState;
        }
    }
}
namespace FiniteStateMachine
{
    [CreateNodeMenu("Transition")]
    public sealed class TransitionNode : FSMNode
    {
        public Decision Decision;
        [Output] public BaseStateNode TrueState;
        [Output] public BaseStateNode FalseState;
        public void Execute(BaseStateMachine stateMachine)
        {
            var trueState = GetFirst<BaseStateNode>(nameof(TrueState));
            var falseState = GetFirst<BaseStateNode>(nameof(FalseState));
            var decision = Decision.Decide(stateMachine);
            if (decision && !(trueState is RemainInStateNode))
            {
                stateMachine.CurrentState = trueState;
            }
            else if(!decision && !(falseState is RemainInStateNode))
                stateMachine.CurrentState = falseState;
        }
    }
}