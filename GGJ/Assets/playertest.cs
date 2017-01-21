using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertest : MonoBehaviour {

/*    Rigidbody2D rigid;
    public Controls control;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag != "Player")
            rigid.velocity += other.gameObject.GetComponent<Rigidbody2D>().velocity / 2;
    }


    void Update() {
        if (Input.GetKey(control.right)) {
            rigid.velocity = new Vector2(5, rigid.velocity.y);
        } else if (Input.GetKey(control.left)) {
            rigid.velocity = new Vector2(-5, rigid.velocity.y);
        }

        if (Input.GetKeyDown(control.jump)) {
            rigid.velocity = new Vector2(rigid.velocity.x, 10);
        }

        if (Input.GetKeyDown(control.down)) {
            rigid.velocity = new Vector2(0, -10);
        }

    }

    float velocity;
    void LateUpdate() {
        velocity = rigid.velocity.y;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag != "Player")
            waveEffect.instance.startForce(Array.IndexOf(waveEffect.instance.segments, other.gameObject), Mathf.Abs(velocity) / 2);
    }*/
}

[Serializable]
public class Controls {
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode down;
}
