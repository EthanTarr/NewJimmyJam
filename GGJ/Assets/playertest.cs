using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertest : MonoBehaviour {

    Rigidbody2D rigid;
    Animator anim;
    public LayerMask groundCheck;
    public Controls control;
    [Space()]
    public float speed = 5;
    public float maxJumpHeight = 10;
    public float minJumpHeight;
    [Space()]
    public GameObject shockWave;

    public float smashSpeed;
    bool smashing;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    float xSpeed;
    void Update() {
        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();
        if (!smashing)
        {
            if (Input.GetKey(control.right))
            {
                xSpeed += speed / 8;
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetFloat("velocity", 1);
            } else if (Input.GetKey(control.left)) {
                xSpeed -= speed / 8;
                GetComponent<SpriteRenderer>().flipX = true;
                anim.SetFloat("velocity", 1);
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * 3);
            }

            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);
            rigid.velocity = new Vector2(xSpeed , rigid.velocity.y);

            if (Input.GetKeyDown(control.jump) && touchingGround) {
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
            }

            if (Input.GetKeyUp(control.jump) && rigid.velocity.y > minJumpHeight) {
                rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
            }


            if (Input.GetKeyDown(control.down)) {
                rigid.velocity = new Vector2(0, -smashSpeed);
                smashing = true;
            }
        }
        anim.SetBool("smashing", smashing);
    }


    public bool checkGround() {
        RaycastHit2D hit = Physics2D.Raycast(GetComponent<Collider2D>().bounds.min, -transform.up, 0.5f, groundCheck);
        Debug.DrawLine(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.min - transform.up * 0.5f);
        anim.SetBool("airborne", hit == false);
        return hit;
    }

    void recover() {
        smashing = false;
    }

    float velocity;
    void LateUpdate() {
        velocity = rigid.velocity.y;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor") && other.relativeVelocity.magnitude > 8) {
            float strength = other.relativeVelocity.magnitude / 50f;
            if (smashing) {
                GameObject shockwave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
                shockwave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, shockwave.GetComponent<SpriteRenderer>().color.a);
                Shake.instance.shake(2, 3);
                smashing = false;
            }
            

            WaveGenerator.instance.makeWave(transform.position.x, strength, GetComponent<SpriteRenderer>().color);
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Floor")) {
            rigid.AddForce(new Vector2(0, other.gameObject.GetComponent<SquareBehavior>().velocity * 1500));
        }
    }
}

[Serializable]
public class Controls {
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode down;
}
