using System.Collections.Generic;
using FiniteStateMachine;
using UnityEngine;

// Note: this class is for makeshift purposes only. DON'T put anything 
// that you want to survive in here!
public abstract class EnemyBehaviour : MonoBehaviour
{    
    private State _state;
    public Transform TargetPlayer {get; protected set;}
    public static readonly HashSet<EnemyBehaviour> instances = new HashSet<EnemyBehaviour>(); // Property
    public static HashSet<EnemyBehaviour> Instances => new HashSet<EnemyBehaviour>(instances); // Field

    public virtual void SwitchToHuntingState(Transform Player)
    {
        _state = State.Hunting;
        TargetPlayer = Player;
    }
    protected virtual void Awake() 
    {
        instances.Add(this);
    }
    protected virtual void OnDestroy()
    {
        instances.Remove(this);
    }
}

enum State 
{
    Searching,
    Hunting,
}