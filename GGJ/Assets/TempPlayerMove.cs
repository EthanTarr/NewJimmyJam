using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerMove : MonoBehaviour {

	public float speed = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.W)) {
			this.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(0, speed * 15));
		} else if (Input.GetKey (KeyCode.A)) {
			this.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(-speed, 0));
		} else if (Input.GetKey (KeyCode.S)) {
			this.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(0, -speed * 5));
		} else if (Input.GetKey (KeyCode.D)) {
			this.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(speed, 0));
		} 
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag.Equals ("Floor") && other.relativeVelocity.magnitude > 8) {
			GameObject.Find ("Main Camera").GetComponent<WaveGenerator> ().makeWave (transform.position.x, other.relativeVelocity.magnitude / 50f);
		}
	}
}
