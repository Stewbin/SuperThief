using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(fileName = "InteractionInputData", menuName = "InteractionSystem/Input")]

    public class InteractionInputData : ScriptableObject
    {



        //Knows when clicked has been pressed
        public bool m_interactedClicked;

        //Knows when clicked has been realeased
        public bool m_interactedReleased;

        public bool InteractedClicked
        {
            get => m_interactedClicked;

            set => m_interactedClicked = value;
        }
        public bool InteractedReleased
        {
            get => m_interactedReleased;

            set => m_interactedReleased = value;
        }

        public void Reset()
        {
            m_interactedClicked = false;
            m_interactedReleased = false;
        }




    }
}


