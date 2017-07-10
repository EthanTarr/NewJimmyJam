using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horseFront : playertest{

    public horseBack back;
    public SpriteRenderer horseHead;

    // Update is called once per frame
    void Update() {
        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();

        bounceDirection = new Vector2(Mathf.Lerp(bounceDirection.x, 0, Time.deltaTime * 2.5f), Mathf.Lerp(bounceDirection.y, 0, Time.deltaTime * (smashing ? 5 : 45)));

        if (!smashing && !vunrabilityFrames && playerControl != "" && rigid.bodyType == RigidbodyType2D.Dynamic) {
            if (Mathf.Abs(Input.GetAxis("Horizontal" + playerControl)) > 0.1f) {
                xSpeed += speed * Input.GetAxis("Horizontal" + playerControl) / (accelerate * 4);
                anim.GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal" + playerControl) < 0;
                horseHead.GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal" + playerControl) < 0;
                horseHead.transform.position = new Vector2(Input.GetAxisRaw("Horizontal" + playerControl) < 0 ? transform.position.x - 0.05f: transform.position.x + 0.05f, horseHead.transform.position.y);
                anim.SetFloat("velocity", 1);
                slopeCheck();
            }
            else
            {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
            }

            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);

            if (touchingGround) {
                transform.Translate(slidingFloor, 0, 0);
            }

            if (Input.GetButtonDown("Jump" + playerControl) && touchingGround)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
                audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.93f, 1.05f));
                Invoke("queueBackJump", 0.15f);
            }

            if (Input.GetButtonUp("Jump" + playerControl) && rigid.velocity.y > minJumpHeight)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
            }

            //if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f) {
            //    StartCoroutine(dashOutOfCharge(Input.GetAxis("Dash" + playerControl), true));
            //}

            if (Input.GetButtonDown("Smash" + playerControl)
                    && canSmash && !smashing && !touchingGround)
            {
                StartCoroutine(chargeSmash(Input.GetAxis("Horizontal" + playerControl)));
            }
        }

        rigid.velocity += gravityDirection * 0.45f;

        if (!smashing && !vunrabilityFrames)
        {
            rigid.velocity = new Vector2(xSpeed, rigid.velocity.y) + bounceDirection;
        }
    }

    public override void die() {
        alreadyDead = true;
        print("hoi");
        audioManager.instance.Play(deathExplosion, 0.5f, UnityEngine.Random.Range(0.96f, 1.04f));
        GameObject particle = Instantiate(deathParticle, transform.position, Quaternion.identity) as GameObject;
        particle.GetComponent<ParticleSystem>().startColor = fullColor;
        GetComponent<SpriteRenderer>().color = fullColor;
        Shake.instance.shake(2, 3);
        Destroy(this.transform.parent.gameObject);
        endingUI.instance.Invoke("checkPlayersLeft", 0.25f);
    }
    void queueBackJump() {
        back.jumpQueued = true;
    }
}
