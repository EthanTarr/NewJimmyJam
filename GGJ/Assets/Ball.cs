using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : playerController {

    protected override void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteAnim = GetComponentInChildren<SpriteScript>();

        active = true;

        changeModifiers();
    }
    public override void CmdinputAudit(float HorizInput)
    {
        checkGround();

        bounceDirection = new Vector2(Mathf.Lerp(bounceDirection.x, 0, Time.deltaTime * (smashing ? 25 : 2.5f)), Mathf.Lerp(bounceDirection.y, 0, Time.deltaTime * (smashing ? 2 : 35)));
        dashDirection = Mathf.Lerp(dashDirection, 0, Time.deltaTime * dashDecel);

        if (!smashing && !vunrabilityFrames && playerControl != "" && rigid.bodyType == RigidbodyType2D.Dynamic)
        {
            movement(HorizInput);
        }
        if (!smashing && !vunrabilityFrames && active)
        {
            rigid.velocity = transform.TransformDirection(new Vector2(xSpeed, transform.InverseTransformDirection(rigid.velocity).y));
        }

        if ((canSmash || !seperateDashCooldown || !tightDash))
        {
            if (centerOfGravity == null)
            {
                rigid.velocity += gravityDirection * gravityStrength;
            }
            else
            {
                Vector2 gravityDirection = (-transform.position + centerOfGravity.position).normalized;

                //rigid.AddForce(gravityDirection * gravityStrength);
                rigid.velocity -= (Vector2)transform.up * gravityStrength;

                Vector2 dirUp = -gravityDirection;
                this.transform.up = Vector2.Lerp(this.transform.up, dirUp, Time.deltaTime * 500);
            }
        }

        rigid.velocity += (Vector2)transform.up * bounceDirection.y;
        if (active)
        {
            rigid.velocity += (Vector2)transform.right * (dashDirection + bounceDirection.x);
        }
    }

    protected override void movement(float horizInput)
    {
        //Horizontal Movement
        touchingGround = checkGround();

        if (active) {
            slopeCheck();
        }
    }

    public override void die()
    {
    }
}
