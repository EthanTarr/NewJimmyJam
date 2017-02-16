using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController {



    public Transform centerOfGravity;

    public Vector2 dirOfGravity;



    public void Move(Vector3 velocity, bool standingOnPlatform = false) { 
        UpdateRaycastOrigins();


        collisions.Reset();

        dirOfGravity = Vector2.zero;

        if (velocity.x != 0)
            HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
            VerticalCollisions(ref velocity);

        CalculateGravity();
        transform.Translate(velocity);

        if (standingOnPlatform) {
            collisions.below = standingOnPlatform;
        }
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

                if (directionY == -1) {
                    collisions.belowCollisions.Add(hit.collider.gameObject);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

 
}
