using XNode;

namespace FiniteStateMachine
{
    [CreateNodeMenu("Initial Node"), NodeTint("#00ff52")]
    public class FSMInitialNode : Node
    {
        [Output] public StateNode InitialNode;
        public StateNode NextNode
        {
            get
            {
                NodePort port = GetOutputPort("InitialNode");
                if (port == null || port.ConnectionCount == 0)
                    return null;
                return port.GetConnection(0).node as StateNode;
            }
        }
    }
}