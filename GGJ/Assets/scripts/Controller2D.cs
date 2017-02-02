using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    public LayerMask collisionMask;

    const float skinWidth = 0.015f;

    public BoxCollider2D col;
    RaycastOrigins raycastOrigin;
    public int horizontalRayCount = 4;
    public int VerticalRayCount = 4;

    float horizontalRaySpacing;
    float VecticalRaySpacing;
    public CollisionInfo collisions;
    public Transform centerOfGravity;

    public Vector2 dirOfGravity;

    void Start() {
        col = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins();
        collisions.Reset();

        dirOfGravity = Vector2.zero;

        if (velocity.x != 0)
            HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
            VerticalCollisions(ref velocity);

        CalculateGravity();
        transform.Translate(velocity);
    }

    void CalculateGravity() {
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.bottomLeft, -transform.up, 1, collisionMask);

        Debug.DrawRay(transform.position, hit.normal, Color.green);
        Debug.DrawRay(transform.position, dirOfGravity, Color.blue);

        if (hit)
            dirOfGravity = -hit.normal;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, -Vector3.Normalize(dirOfGravity));
        
    }

    void HorizontalCollisions(ref Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < VerticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigin.bottomLeft : raycastOrigin.topRight;
            rayOrigin += (Vector2)transform.up * -directionX * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.right * directionX, rayLength, collisionMask);

            //Debug.DrawRay(rayOrigin, directionX * transform.right * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < VerticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigin.bottomLeft : raycastOrigin.topRight;
            rayOrigin += (Vector2)(-directionY * transform.right) * (VecticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.up * directionY, rayLength, collisionMask);

            //Debug.DrawRay(rayOrigin, transform.up * 10 * rayLength, Color.red);

            if (hit) {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        Vector3 boundsMax = transform.TransformPoint(col.offset + col.size / 2);
        Vector3 boundsMin = transform.TransformPoint(col.offset - col.size / 2);

        raycastOrigin.bottomLeft = new Vector2(boundsMin.x, boundsMin.y);
        raycastOrigin.BottomRight = new Vector2(boundsMax.x, boundsMin.y);
        raycastOrigin.topLeft = new Vector2(boundsMin.x, boundsMax.y);
        raycastOrigin.topRight = new Vector2(boundsMax.x, boundsMax.y);
    }

    void CalculateRaySpacing() {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        VerticalRayCount = Mathf.Clamp(VerticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        VecticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, BottomRight;
    }

    public struct CollisionInfo{
    public bool above, below, left, right;

        public void Reset() {
            above = below = false;
            left = right = false;
        }
    }
}
