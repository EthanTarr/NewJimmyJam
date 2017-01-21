using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiPulseMove : MonoBehaviour {

	public float speed = 5;
	public float Amplitude = 1;
    public Color color = Color.white;
	private bool forward = true;

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (transform.position.x > -GameManager.boundary && forward) {
			transform.Translate (new Vector3 (Time.deltaTime * -speed, 0, 0));
		} else if (forward) {
			forward = false;
			if (Amplitude < .1f) {
				Destroy (this.gameObject);
			}
			Amplitude = Amplitude / 4;
        } else if (!forward && transform.position.x < GameManager.boundary) {
			transform.Translate (new Vector3 (Time.deltaTime * speed, 0, 0));
		} else if (!forward) {
			forward = true;
			if (Amplitude < .1f) {
				Destroy (this.gameObject);
			}
			Amplitude = Amplitude / 4;
        }
        this.color = Color.Lerp(color, Color.white, Time.deltaTime / 32);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<SquareBehavior>() != null)
        {
            other.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
