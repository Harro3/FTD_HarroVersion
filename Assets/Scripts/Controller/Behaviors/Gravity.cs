using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public class Gravity : Behavior
    {
        [SerializeField] private float fallClamp_ = -40f;
        [SerializeField] private float minFallSpeed_ = 80f;
        [SerializeField] private float maxFallSpeed_ = 120f;

        private float fallSpeed_ = 0f;
        private float apexPoint_ = 0f;

        public override void UpdateBehavior()
        {
            if (!controller.collideDown)
            {
                apexPoint_ = Mathf.InverseLerp(controller.jumpApexThreshold, 0, Mathf.Abs(controller.velocity.y));
                fallSpeed_ = Mathf.Lerp(minFallSpeed_, maxFallSpeed_, apexPoint_);
            }

            if (controller.collideDown)
            {
                if (controller.velocity.y < 0)
                {
                    controller.velocity.y = 0;
                }
            }
            else
            {
                var fallSpeed = controller.endedJumpEarly && controller.velocity.y > 0 ? fallSpeed_ * controller.jumpEndEarlyGravityModifier : fallSpeed_;

                controller.velocity.y -= fallSpeed * Time.deltaTime;

                if (controller.velocity.y < fallClamp_)
                {
                    controller.velocity.y = fallClamp_;
                }
            }
        }
    }
}
