using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Floor"){
			this.GetComponent<PolygonCollider2D> ().enabled = false;
            GetComponent<Rigidbody2D>().velocity += Vector2.up * 1.5f;
            WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, 0.75f, Color.white, 3, null);
        }
	}
}
