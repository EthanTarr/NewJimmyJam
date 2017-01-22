using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour {

	public GameObject shockWave;
	public GameObject deathParticle;

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.GetComponent<playertest> () != null) {
			GameObject particle = Instantiate (deathParticle, other.gameObject.transform.position, other.gameObject.transform.rotation) as GameObject;
			GameObject shockwaye = Instantiate (shockWave, other.gameObject.transform.position, other.gameObject.transform.rotation) as GameObject;

			shockwaye.GetComponent<SpriteRenderer> ().color = other.gameObject.GetComponent<SpriteRenderer> ().color;
			particle.GetComponent<ParticleSystem> ().startColor = other.gameObject.GetComponent<SpriteRenderer> ().color;

			Destroy (other.gameObject);

		} else {
			this.GetComponent<BoxCollider2D> ().enabled = false;
		}
	}
}
