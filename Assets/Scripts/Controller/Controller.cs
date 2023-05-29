using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public class Controller : MonoBehaviour
    {

        [Header("Collider")]
        public Bounds bounds;
        public LayerMask groundLayer;
        public bool collideUp = false;
        public bool collideDown = false;
        public bool collideLeft = false;
        public bool collideRight = false;


        [Header("Jump")]
        public bool landingThisFrame = false;
        public bool coyoteUsable = false;
        public bool endedJumpEarly = false;
        public float jumpEndEarlyGravityModifier = 3;
        public float jumpApexThreshold = 10f;

        [Header("Movement")]
        public Vector2 velocity;
        [SerializeField] private int freeColliderIterations_ = 10;

        private Behavior[] behaviors_;

        private bool active_;
        void Awake() => Invoke(nameof(Activate), 0.5f);
        void Activate() =>  active_ = true;

        void Start()
        {
            behaviors_ = GetComponents<Behavior>();
            if (behaviors_ == null)
                Debug.LogError("Behaviors not found");
        }

        void Update()
        {
            if (!active_) return;

            foreach (Behavior behavior in behaviors_)
            {
                behavior.UpdateBehavior();
            }

            var pos = transform.position + bounds.center;
            Vector3 move = velocity * Time.deltaTime;
            var furthestPoint = pos + move;

            var hit = Physics2D.OverlapBox(furthestPoint, bounds.size, 0, groundLayer);
            if (!hit)
            {
                transform.position += move;
                return;
            }

            var posToMoveTo = transform.position;

            for (int i = 0; i < freeColliderIterations_; i++)
            {
                var t = (float)i / freeColliderIterations_;
                
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, bounds.size, 0, groundLayer))
                {
                    transform.position = posToMoveTo;
                    if (i == 1)
                    {
                        if (velocity.y < 0)
                        {
                            velocity.y = 0;
                        }
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }
                    return;
                }

                posToMoveTo = posToTry;
            }
        
        }
    }
}
