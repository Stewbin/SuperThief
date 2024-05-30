using System.Linq;
using UnityEngine;
using XNode;

namespace FiniteStateMachine
{
    [CreateAssetMenu(menuName = "FSM/FSM Graph")]
    public sealed class FSMGraph : NodeGraph
    {
        private StateNode _initialState;
        public StateNode InitialState
        {
            get
            {
                if (_initialState == null)
                    _initialState = FindInitialStateNode();
                return _initialState;
            }
        }
        private StateNode FindInitialStateNode()
        {
            var initialNode = nodes.FirstOrDefault(x => x is FSMInitialNode);
            if (initialNode != null)
            {
                return (initialNode as FSMInitialNode).NextNode;
            }
            return null;
        }
    }
}