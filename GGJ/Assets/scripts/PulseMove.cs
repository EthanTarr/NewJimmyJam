using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseMove : MonoBehaviour {

	public float speed = 5;
    public float angularSpeed = 20;
	public float Amplitude = 1;
    public Color color = Color.white;
    public Transform centerOfGravity;
	private bool forward = true;
    public AudioClip roll;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
<<<<<<< HEAD
		if (transform.position.x < GameManager.boundary && forward) {
			transform.Translate (new Vector3 (Time.deltaTime * speed, 0, 0));
		} else if (forward) {
			forward = false;
			if (Amplitude < 1f) {
				Destroy (this.gameObject);
			}
			//Amplitude = Amplitude / 4;
		} else if (!forward && transform.position.x > -GameManager.boundary) {
            transform.Translate(new Vector3(Time.deltaTime * speed, 0, 0));

        } else if (!forward) {
			forward = true;
			if (Amplitude < 1f) {
				Destroy (this.gameObject);
			}
			//Amplitude = Amplitude / 4;
=======
        if (centerOfGravity.GetComponent<TerrainGenerator>().shape == TerrainGenerator.Shape.Plane) {
            if (transform.position.x < GameManager.boundary && forward)
            {
                transform.Translate(new Vector3(Time.deltaTime * speed, 0, 0));
            }
            else if (forward)
            {
                forward = false;
                if (Amplitude < 1f)
                {
                    Destroy(this.gameObject);
                }
                Amplitude = Amplitude / 4;
            }
            else if (!forward && transform.position.x > -GameManager.boundary)
            {
                transform.Translate(new Vector3(Time.deltaTime * -speed, 0, 0));
            }
            else if (!forward)
            {
                forward = true;
                if (Amplitude < 1f)
                {
                    Destroy(this.gameObject);
                }
                Amplitude = Amplitude / 4;
            }
            this.color = Color.Lerp(color, Color.white, Time.deltaTime / 32);
        } else if (centerOfGravity.GetComponent<TerrainGenerator>().shape == TerrainGenerator.Shape.Sphere) {
            transform.RotateAround(centerOfGravity.position, new Vector3(0, 0, 1), angularSpeed * Time.deltaTime);
>>>>>>> origin/Jose
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<SquareBehavior>() != null) {
            if (!other.gameObject.GetComponent<SquareBehavior>().firstBlock) {
                audioManager.instance.Play(roll, 0.25f);
            }
            other.gameObject.GetComponent<SquareBehavior>().firstBlock = true;
            other.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
