using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VHS
{
    public class InteractionController : MonoBehaviour
    {
        [Header("Data")]
        public InteractionInputData interactionInputData = null;
        public InteractionData interactionData = null;

        [Space, Header("UI")]
        [SerializeField] private InteractionUIPanel uiPanel; 



        [Space]
        [Header("Ray Settings")]
        public float rayDistance;
        public float raySphereRadius;
        public LayerMask interactableLayer;

        private Camera m_cam;

        private bool m_interacting; 

        private float m_holdTimer = 0f;

        void Awake()
        {
            m_cam = FindObjectOfType<Camera>();
        }

        void Update()
        {
            CheckForInteractable();
            CheckForInteractableInput();
        }

        void CheckForInteractable()
        {
            Ray _ray = new Ray(m_cam.transform.position, m_cam.transform.forward);
            RaycastHit _hitInfo;

            bool _hitSomething = Physics.SphereCast(_ray, raySphereRadius, out _hitInfo, rayDistance, interactableLayer);

            if (_hitSomething)
            {
                InteractableBase _interactable = _hitInfo.transform.GetComponent<InteractableBase>();

                if (_interactable != null)
                {
                    if (interactionData.IsEmpty())
                    {
                        interactionData.Interactable = _interactable;
                        uiPanel.SetTooltip("Interact");
                    }
                    else
                    {
                        if (!interactionData.IsSameInteractable(_interactable))
                        {
                            interactionData.Interactable = _interactable;
                            uiPanel.SetTooltip("Interact");

                        }
                       
                    }
                }
            }
            else
            {
                uiPanel.ResetUI();

                interactionData.ResetData();
            }

            // Uncomment the line below to see the ray in the Scene view for debugging
             Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, _hitSomething ? Color.green : Color.red);
        }

        void CheckForInteractableInput()
        {
            // Implementation of interaction input checking
            if(interactionData.IsEmpty()){
                return; 
            } 

            if(interactionInputData.InteractedClicked){
                
                m_interacting = true; 
                m_holdTimer = 0f; 

            }

            if(interactionInputData.InteractedReleased){
                m_interacting = false; 
                m_holdTimer = 0f; 
            }

            if(m_interacting){

                if(!interactionData.Interactable.IsInteractable){
                    return ; 
                }

                 if(interactionData.Interactable.HoldInteract){
                    m_holdTimer += Time.deltaTime; 

                    float heldPercent = m_holdTimer / interactionData.Interactable.HoldDuration;
                    uiPanel.UpdateProgressBar(heldPercent);


                    if (heldPercent > 1f){
                        interactionData.Interact(); 
                        m_interacting = false; 
                    }
                }
            } else {
                interactionData.Interact(); 
                m_interacting = false; 
            }
        }

    }
}
