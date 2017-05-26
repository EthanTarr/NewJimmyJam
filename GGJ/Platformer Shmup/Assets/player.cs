using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    public float speed = 1;
    public GameObject bullet;

    private Rigidbody2D rigid;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
    }

	void Update () {
        rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rigid.velocity.y);


        if (Input.GetKeyDown(KeyCode.Space))  {
            Instantiate(bullet, transform.position, transform.rotation);
        }
	}
}
