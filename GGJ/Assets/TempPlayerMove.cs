using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerMove : MonoBehaviour {

	public float speed = 5;
    public float jumpHeight = 3;

    Rigidbody2D rigid;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.W)) {
			rigid.velocity =  new Vector2(0, jumpHeight);
		} else if (Input.GetKey (KeyCode.A)) {
            rigid.velocity = new Vector2(-speed, 0);
		} else if (Input.GetKey (KeyCode.S)) {
            rigid.AddForce (new Vector2(0, -speed * 5));
		} else if (Input.GetKey (KeyCode.D)) {
			rigid.AddForce (new Vector2(speed, 0));
		} 
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag.Equals ("Floor") && other.relativeVelocity.magnitude > 8) {
			GameObject.Find ("Main Camera").GetComponent<WaveGenerator> ().makeWave (transform.position.x, other.relativeVelocity.magnitude / 100);
		}
	}
}
