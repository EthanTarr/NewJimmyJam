﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AntiPulseMove : NetworkBehaviour
{

    [SyncVar]  public float speed = 5;
    [SyncVar]  public float angularSpeed = 5;
    [SyncVar]  public float Amplitude = 1;
    [SyncVar]  public Color color = Color.white;
    [SyncVar]  public Transform centerOfGravity;
    [SyncVar]  private bool forward = true;
    public bool isOnline;

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {
        if (centerOfGravity == null) {
            if (transform.position.x > -WaveManager.boundary && forward) {
                transform.Translate(new Vector3(Time.deltaTime * -speed, 0, 0));
            } else if (forward) {
                forward = false;
                if (Amplitude < .1f) {
                    Destroy(this.gameObject);
                }
                Amplitude = Amplitude / 4;
            }else if (!forward && transform.position.x < WaveManager.boundary){
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
                Amplitude = Amplitude / 4;
            }
            this.color = Color.Lerp(color, Color.white, Time.deltaTime / 32);
        } else {
            transform.RotateAround(centerOfGravity.position, new Vector3(0, 0, 1), -angularSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<SquareBehavior>() != null)
        {
            other.gameObject.GetComponent<SquareBehavior>().firstBlock = true;
            other.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
