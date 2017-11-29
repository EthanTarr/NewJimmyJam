using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiPulseMove : MonoBehaviour
{

    public float speed = 5;
    public float angularSpeed = 5;
    public float Amplitude = 1;
    public Color color = Color.white;
    public Transform centerOfGravity;
    private bool forward = true;
    public bool isOnline;

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {

        if (centerOfGravity == null) {
            if (transform.position.x > -TerrainGenerator.boundary && forward) {
                transform.Translate(new Vector3(Time.deltaTime * -speed, 0, 0));
            } else if (forward) {
                forward = false;
                if (Amplitude < .1f) {
                    Destroy(this.gameObject);
                }
                //Amplitude = Amplitude / 4;
            }else if (!forward && transform.position.x < TerrainGenerator.boundary){
                
                transform.Translate(new Vector3(Time.deltaTime * speed, 0, 0));
                GameObject Pulse = Instantiate(WaveGenerator.instance.pulse, transform.position, Quaternion.identity);
                Pulse.GetComponent<PulseMove>().color = color;
                Pulse.GetComponent<PulseMove>().Amplitude = Amplitude / 2;
                Pulse.GetComponent<PulseMove>().speed = speed / 2;
                
                Destroy(this.gameObject);
                
            } else if (!forward) {
                forward = true;
                if (Amplitude < .1f) {
                    Destroy(this.gameObject);
                }
                //Amplitude = Amplitude / 4;
            }
            //this.color = Color.Lerp(color, Color.white, Time.deltaTime / 32);
        } else {
            if (Amplitude < .1f || angularSpeed < 10) {
                Destroy(this.gameObject);
            }

            transform.RotateAround(centerOfGravity.position, new Vector3(0, 0, 1), -angularSpeed * Time.deltaTime);
        }
    }

    bool first;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<SquareBehavior>() != null) {
            other.gameObject.GetComponent<SquareBehavior>().firstBlock = true;
            //other.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
        else if (other.GetComponent<PulseMove>() != null) {
            if (!first) {
                first = true;
            } else if(Mathf.Abs(other.GetComponent<PulseMove>().Amplitude - Amplitude) <= 1f) {
                //print("hoi1");
                other.GetComponent<PulseMove>().Amplitude /= 2;
                Amplitude /= 2;
                other.GetComponent<PulseMove>().speed /= 1.75f;
                speed /= 1.75f;
                angularSpeed /= 1.5f;
                other.GetComponent<PulseMove>().angularSpeed /= 1.5f;

                if (Amplitude < .1f)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
