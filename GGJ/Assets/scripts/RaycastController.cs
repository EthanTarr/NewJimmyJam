using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour {

    public LayerMask collisionMask;
    public CollisionInfo collisions;

    protected const float skinWidth = 0.015f;

    public BoxCollider2D col;
    protected RaycastOrigins raycastOrigin;
    public int horizontalRayCount = 4;
    public int VerticalRayCount = 4;

    protected float horizontalRaySpacing;
    protected float VecticalRaySpacing;

    public virtual void Start() {
        collisions.belowCollisions = new List<GameObject>();
        col = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    protected void UpdateRaycastOrigins() {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        Vector3 boundsMax = transform.TransformPoint(col.offset + col.size / 2);
        Vector3 boundsMin = transform.TransformPoint(col.offset - col.size / 2);

        raycastOrigin.bottomLeft = new Vector2(boundsMin.x, boundsMin.y);
        raycastOrigin.BottomRight = new Vector2(boundsMax.x, boundsMin.y);
        raycastOrigin.topLeft = new Vector2(boundsMin.x, boundsMax.y);
        raycastOrigin.topRight = new Vector2(boundsMax.x, boundsMax.y);
    }

    protected void CalculateRaySpacing() {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        VerticalRayCount = Mathf.Clamp(VerticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        VecticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
    }

    protected struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, BottomRight;
    }

    public struct CollisionInfo{
        public bool above, below, left, right;
        public List<GameObject> belowCollisions;

        public void Reset() {
            above = below = false;
            left = right = false;
            belowCollisions = new List<GameObject>();
        }
    }
}
