using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace FiniteStateMachine
{   
    /// <summary>
    /// Parent class all nodes in the FSM namespace inherit from
    /// </summary>
    public abstract class FSMNode : Node
    {
        [Input(backingValue = ShowBackingValue.Never)] public FSMNode Entry;

        protected IEnumerable<T> GetAllOnPort<T>(string fieldName) where T : FSMNode
        {
            NodePort port = GetOutputPort(fieldName);
            for (var portIndex = 0; portIndex < port.ConnectionCount; portIndex++)
            {
                yield return port.GetConnection(portIndex).node as T;
            }
        }

        protected T GetFirst<T>(string fieldName) where T : FSMNode
        {
            NodePort port = GetOutputPort(fieldName);
            if (port.ConnectionCount > 0)
                return port.GetConnection(0).node as T;
            return null;
        }
    }
}