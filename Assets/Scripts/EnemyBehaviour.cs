using System;
using System.Collections.Generic;
using FiniteStateMachine;
using Photon.Pun;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Note: this class is for makeshift purposes only. DON'T put anything 
// that you want to survive in here!

[Serializable]
public abstract class EnemyBehaviour : MonoBehaviour
{   
    [SerializeField] public State CurrentState {get; protected set;}
    [SerializeField] public Transform TargetPlayer {get; protected set;}
    private static readonly HashSet<EnemyBehaviour> _instances = new HashSet<EnemyBehaviour>(); // Property
    public static HashSet<EnemyBehaviour> Instances => new(_instances); // Field

    public virtual void SwitchToHuntingState(Transform Player)
    {
        CurrentState = State.Hunting;
        TargetPlayer = Player;
    }
    protected virtual void Awake() 
    {
        _instances.Add(this);
    }
    protected virtual void OnDestroy()
    {
        _instances.Remove(this);
    }

    public enum State 
    {
        Searching,
        Hunting,
    }

}
