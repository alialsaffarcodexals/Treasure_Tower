using UnityEngine;

namespace TreasureTower.Systems
{
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.18f;
        [SerializeField] private Vector2 minBounds = new(-10f, -10f);
        [SerializeField] private Vector2 maxBounds = new(10f, 10f);

        private Vector3 velocity;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }
}
