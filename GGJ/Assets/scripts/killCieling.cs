using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killCieling : MonoBehaviour {

    public GameObject deathParticle;
    public AudioClip deathExplosion;


    void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.GetComponent<playertest> () != null) {
            audioManager.instance.Play(deathExplosion, 0.5f, Random.Range(0.96f, 1.04f));
            GameObject particle = Instantiate (deathParticle, other.gameObject.transform.position, other.gameObject.transform.rotation) as GameObject;
            particle.GetComponent<ParticleSystem>().startColor = other.gameObject.GetComponent<playertest>().fullColor;
            GetComponent<SpriteRenderer>().color = other.gameObject.GetComponent<playertest>().fullColor;
            Shake.instance.shake(2, 3);
            audioManager.instance.Play(deathExplosion, 0.75f, Random.Range(0.96f, 1.04f));

            Destroy(other.gameObject);
            endingUI.instance.Invoke("checkPlayersLeft", 0.5f);
            //endingUI.instance.checkPlayersLeft();   
        }
    }
}
