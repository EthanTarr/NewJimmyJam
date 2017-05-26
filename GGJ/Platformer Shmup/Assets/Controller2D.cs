using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    public LayerMask collisionMask;
    public LayerMask gravityMask;

    const float skinWidth = 0.015f;

    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigin;
    public int horizontalRayCount = 4;
    public int VerticalRayCount = 4;

    float horizontalRaySpacing;
    float VecticalRaySpacing;
    public CollisionInfo collisions;
    [HideInInspector] public Vector3 centerOfGravity;

    [HideInInspector] public Vector2 dirOfGravity;

    void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        collisions.Reset();
        CalculateRaySpacing();
    }

    // Takes in an objects velocity, resets and checks for collision, 
    // sets direction of gravity and then moves the object
    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0)
            HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
            VerticalCollisions(ref velocity);

        //dirOfGravity = centerOfGravity - transform.position;

        CalculateGravity();
        transform.Translate(velocity);
    }

    // calculates direction of gravity when it is close enough to the ground
    // sets the player gravity angle to the ground
    // TODO: make it so there is a maximum angle compared to the initial direction of gravity 
    // TODO: possibly combine this with VerticalCollisions()
    void CalculateGravity() {
        // handles readujusting angle to collision when it collides with a surface

        RaycastHit2D hit1 = Physics2D.Raycast(raycastOrigin.bottomLeft, -transform.up, 0.5f, gravityMask);
        RaycastHit2D hit2 = Physics2D.Raycast(raycastOrigin.BottomRight, -transform.up, 0.5f, gravityMask);

        // Debug.DrawRay(raycastOrigin.bottomLeft, -transform.up, Color.green);
        // Debug.DrawRay(raycastOrigin.BottomRight, -transform.up, Color.green);
        // Debug.DrawRay(transform.position, dirOfGravity, Color.blue);

        Vector2 floorAngle = new Vector2(1000, 1000);
        if (hit1 && hit2) {
            Vector3 gangle = hit1.point - hit2.point;
            floorAngle = Vector3.Cross(transform.forward, gangle);
        } else if (hit1) {
            floorAngle = -hit1.normal;
        } else if (hit2) {
            floorAngle = -hit2.normal;
        }

        if (floorAngle != new Vector2(1000, 1000) && Vector2.Angle(floorAngle, dirOfGravity) < 50) {
            dirOfGravity = floorAngle;
        }

        // rotates object toward direction of gravity
        Quaternion angle = Quaternion.FromToRotation(Vector3.up, -Vector3.Normalize(dirOfGravity)); 
        if (collisions.below) {
            transform.rotation = angle;
        } else {
            transform.rotation = Quaternion.Lerp(transform.rotation, angle, Time.deltaTime * 10);
        }
    }

    void HorizontalCollisions(ref Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < VerticalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigin.bottomLeft : raycastOrigin.topRight;
            rayOrigin += (Vector2)transform.up * -directionX * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.right * directionX, rayLength, collisionMask);

            //Debug.DrawRay(rayOrigin, directionX * transform.right * rayLength, Color.red);
            if (hit) {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
                collisions.collisionObjects.Add(hit.transform.gameObject);
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
                collisions.collisionObjects.Add(hit.transform.gameObject);
            }
        }
    }

    void UpdateRaycastOrigins() {
        /*Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        Vector3 boundsMax = transform.TransformPoint(collider.offset + collider.size / 2);
        Vector3 boundsMin = transform.TransformPoint(collider.offset - collider.size / 2);

        raycastOrigin.bottomLeft = new Vector2(boundsMin.x, boundsMin.y);
        raycastOrigin.BottomRight = new Vector2(boundsMax.x, boundsMin.y);
        raycastOrigin.topLeft = new Vector2(boundsMin.x, boundsMax.y);
        raycastOrigin.topRight = new Vector2(boundsMax.x, boundsMax.y); */

        Vector2 center = boxCollider.offset;
        Vector2 extents = boxCollider.size * 0.5f;

        raycastOrigin.bottomLeft = transform.TransformPoint(center +  new Vector2(-extents.x, -extents.y));
        raycastOrigin.BottomRight = transform.TransformPoint(center +  new Vector2(extents.x, -extents.y));
        raycastOrigin.topLeft = transform.TransformPoint(center +  new Vector2(-extents.x, extents.y));
        raycastOrigin.topRight = transform.TransformPoint(center +  new Vector2(extents.x, extents.y)); 
    }

    void CalculateRaySpacing() {
        Bounds bounds = boxCollider.bounds;
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
    public List<GameObject> collisionObjects;

        public void Reset() {
            above = below = false;
            left = right = false;
            collisionObjects = new List<GameObject>();
        }
    }
}
