using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CharacterController
{
    public class Collider : Behavior
    {
        private struct RayRange
        {
            public readonly Vector2 start, end, dir;

            public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
            {
                start = new Vector2(x1,y1);
                end = new Vector2(x2,y2);
                this.dir = dir;
            }
        }

        [SerializeField] private int detectorCount_ = 3;
        [SerializeField] private float detectionRayLength_ = 0.1f;
        [SerializeField] [Range(0.1f, 0.3f)] private float raysBuffer = 0.1f;

        // ray ranges (up, down, left, right)
        private Dictionary<string, RayRange> rayRanges_ = new Dictionary<string, RayRange>();

        private void CalculateRayRanges()
        {
            var b = new Bounds(transform.position + controller.bounds.center, controller.bounds.size);

            rayRanges_["down"] = new RayRange(b.min.x + raysBuffer, b.min.y, b.max.x - raysBuffer, b.min.y, Vector2.down);
            rayRanges_["up"] = new RayRange(b.min.x + raysBuffer, b.max.y, b.max.x - raysBuffer, b.max.y, Vector2.up);
            rayRanges_["left"] = new RayRange(b.min.x, b.min.y + raysBuffer, b.min.x, b.max.y - raysBuffer, Vector2.left);
            rayRanges_["right"] = new RayRange(b.max.x, b.min.y + raysBuffer, b.max.x, b.max.y - raysBuffer, Vector2.right);
        }

        private IEnumerable<Vector2> EvaluatePositions(RayRange range)
        {
            for (var i = 0; i < detectorCount_; i++)
            {
                var t = (float)i / (detectorCount_ - 1);
                yield return Vector2.Lerp(range.start, range.end, t);
            }
        }

        private bool CheckCollision(string direction)
        {
            return EvaluatePositions(rayRanges_[direction]).Any(point => Physics2D.Raycast(point, rayRanges_[direction].dir, detectionRayLength_, controller.groundLayer).collider != null);
        }

        public override void UpdateBehavior()
        {
            CalculateRayRanges();

            controller.landingThisFrame = false;

            var groundCheck = CheckCollision("down");

            if (controller.collideDown && !groundCheck)
            {
                var timeLeftGrounded = Time.time;
            }
            else if (!controller.collideDown && groundCheck)
            {
                controller.coyoteUsable = true;
                controller.landingThisFrame = true;
            }

            controller.collideDown = groundCheck;

            controller.collideUp = CheckCollision("up");
            controller.collideLeft = CheckCollision("left");
            controller.collideRight = CheckCollision("right");
        }

        private void OnDrawGizmos()
        {
            if (!controller)
                controller = GetComponent<Controller>();
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + controller.bounds.center, controller.bounds.size);

            if (!Application.isPlaying)
                return;

            Gizmos.color = Color.red;
            Vector3 move = new Vector3(controller.velocity.x * Time.deltaTime, controller.velocity.y * Time.deltaTime);
            Gizmos.DrawWireCube(transform.position + controller.bounds.center + move, controller.bounds.size);
        }
    }


}
