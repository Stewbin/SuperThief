using UnityEngine;
using FiniteStateMachine;
using System;
using System.Collections.Generic;

public class BaseStateMachine : MonoBehaviour 
    {
        [SerializeField] private BaseState _initialState;
        public BaseState CurrentState { get; set; }
        private Dictionary<Type, Component> _cachedComponents;
        
        void Awake()
        {
            Init();
            _cachedComponents = new Dictionary<Type, Component>();
        }

        void Update()
        {
            Execute();
        }

        public virtual void Init()
        {
            CurrentState = _initialState;
        }

        public virtual void Execute()
        {
            CurrentState.Execute(this);
        }

        /// <summary>
        /// Custom GetComponent method that will 
        /// return previously fetched components from cache instead of 
        /// accessing like normal. (The tutorial recommends this).
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <returns>T component attached to game object.</returns>
        public new T GetComponent<T>() where T : Component
        {
            if(_cachedComponents.ContainsKey(typeof(T)))
                return _cachedComponents[typeof(T)] as T;

            var component = base.GetComponent<T>();
            if(component != null)
            {
                _cachedComponents.Add(typeof(T), component);
            }
            return component;
        }
    }