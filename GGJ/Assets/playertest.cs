﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertest : MonoBehaviour {

    Rigidbody2D rigid;
    public LayerMask groundCheck;
    public Controls control;
    [Space()]
    public float speed = 5;
    public float jumpHeight = 10;
	public float bounceForce = 100;
    Animator anim;
    bool smashing;
	private int jumps = 0;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update() {
        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();
		if (touchingGround) {
			jumps = 1;
		}
        if (!smashing)
        {
            if (Input.GetKey(control.right))
            {
                rigid.velocity = new Vector2(speed, rigid.velocity.y);
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetFloat("velocity", 1);
            }
            else if (Input.GetKey(control.left))
            {
                rigid.velocity = new Vector2(-speed, rigid.velocity.y);
                GetComponent<SpriteRenderer>().flipX = true;
                anim.SetFloat("velocity", 1);
            }


			if (Input.GetKeyDown(control.jump) && jumps > 0)
            {
				jumps--;
				rigid.velocity = new Vector2(rigid.velocity.x, jumpHeight);
            }

            if (Input.GetKeyDown(control.down))
            {
                rigid.velocity = new Vector2(0, -20);
                smashing = true;
            }
        }
        anim.SetBool("smashing", smashing);
		if (touchingGround) {
			checkForWave ();
		}
    }
		
	void checkForWave() {
		foreach (GameObject square in GameObject.FindGameObjectsWithTag("Floor")) {
			if (Mathf.Abs (square.transform.position.x - transform.position.x) < .5f && square.GetComponent<SquareBehavior> ().TotalAmplitude > 1) {
				rigid.AddForce (new Vector2(0, square.GetComponent<SquareBehavior> ().TotalAmplitude * bounceForce));
			}
		}
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
            float strength = other.relativeVelocity.magnitude / 10f;
            if (smashing) {
                strength *= 1;
                Shake.instance.shake(2, 3);
            }
            

            WaveGenerator.instance.makeWave(transform.position.x, strength, GetComponent<SpriteRenderer>().color);
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Floor")) {
            rigid.AddForce(new Vector2(0, other.gameObject.GetComponent<SquareBehavior>().velocity * 1500));
            recover();
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
