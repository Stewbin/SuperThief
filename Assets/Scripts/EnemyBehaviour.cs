using System.Collections.Generic;
using FiniteStateMachine;
using UnityEngine;
public abstract class EnemyBehaviour : MonoBehaviour
{    
    private FSMGraph _graph;
    public abstract void SwitchToHuntingState(Transform Player);
    private static readonly HashSet<EnemyBehaviour> instances = new HashSet<EnemyBehaviour>(); // Property
    public static HashSet<EnemyBehaviour> Instances => new HashSet<EnemyBehaviour>(instances); // Field

    protected virtual void Awake() 
    {
        instances.Add(this);
    }
    protected virtual void OnDestroy()
    {
        instances.Remove(this);
    }
}