using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public abstract class Behavior : MonoBehaviour
    {
        protected Controller controller;

        void Start()
        {
            controller = GetComponent<Controller>();
            if (controller == null)
                Debug.LogError("Controller not found");
        }

        public abstract void UpdateBehavior();
    }
}
