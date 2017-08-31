﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horseBack : playerController {

    [Space()]
    public horseFront front;
    public float followDistance;
    public bool jumpQueued;
    public GameObject tail;
    public GameObject[] allObjects;

    void Start() {
        setSettings();
        base.Start();
    }

    void setSettings() {
        this.GetComponent<SpriteRenderer>().color = front.GetComponent<SpriteRenderer>().color;
        this.playerControl = front.playerControl;
        this.fullColor = front.fullColor;
        foreach (GameObject segment in allObjects) {
            segment.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        }
    }

    protected override void movement(float horizInput) {
        bool touchingGround = checkGround();
        if (!smashing && !vunrabilityFrames && playerControl != "" && rigid.bodyType == RigidbodyType2D.Dynamic) {
            Vector2 targetLocation = front.transform.position - front.transform.right * followDistance * (front.GetComponent<horseFront>().spriteAnim.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            float distance = targetLocation.x - transform.position.x;

            if (Mathf.Abs(distance) > 0.1f) {
                horizontalMovement(Mathf.Sign(distance));
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
            }
        }
        if (front.transform.position.y < transform.position.y && rigid.velocity.y > minJumpHeight) {
            rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
        }

        if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && canDash && canSmash && (!touchingGround || canDashOnGround)) {
            StartCoroutine(dash(Input.GetAxis("Dash" + playerControl), true, false));
        }

        if (Input.GetButtonDown("Smash" + playerControl)
                && canSmash && !smashing && !touchingGround) {
            StartCoroutine(chargeSmash(Input.GetAxis("Horizontal" + playerControl)));
        }

        if (jumpQueued && touchingGround)
        {
            startJump();
        }

        spriteAnimationManager(xSpeed, touchingGround);
    }

    public void startJump() {
        rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
        audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.93f, 1.05f));
        jumpQueued = false;
    }

    public void stopJump() {
        if (rigid.velocity.y > minJumpHeight) {
            rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
        }
    }

    void horizontalMovement(float horizAxis) {
        if (Mathf.Abs(horizAxis) > 0.1f) {
            xSpeed += speed * horizAxis / (accelerate * 4);
            spriteAnim.GetComponent<SpriteRenderer>().flipX = horizAxis < 0;
            tail.GetComponent<SpriteRenderer>().flipX = horizAxis > 0;
            tail.transform.position = new Vector2(horizAxis < 0 ? transform.position.x + 0.35f : transform.position.x - 0.35f, tail.transform.position.y);
            slopeCheck();
        } else {
            xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
        }

        xSpeed = Mathf.Clamp(xSpeed, -speed, speed);
    }

    public override void die() {
        alreadyDead = true;
        audioManager.instance.Play(deathExplosion, 0.5f, UnityEngine.Random.Range(0.96f, 1.04f));
        GameObject particle = Instantiate(deathParticle, transform.position, Quaternion.identity) as GameObject;
        particle.GetComponent<ParticleSystem>().startColor = fullColor;
        GetComponent<SpriteRenderer>().color = fullColor;
        Shake.instance.shake(2, 3);
        Destroy(this.transform.parent.gameObject);
        endingUI.instance.Invoke("checkPlayersLeft", 0.25f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector2 targetLocation =  front.transform.position - front.transform.right * followDistance * (front.GetComponent<horseFront>().spriteAnim.GetComponent<SpriteRenderer>().flipX ? -1 : 1);

        Gizmos.DrawWireSphere(targetLocation, 0.25f);
    }
}
