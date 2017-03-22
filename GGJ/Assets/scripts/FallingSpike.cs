using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Floor"){
			this.GetComponent<BoxCollider2D> ().enabled = false;
		}
	}
}
