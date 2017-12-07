﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PulseMove : NetworkBehaviour{

    [SyncVar]  public float speed = 5;
    [SyncVar]  public float angularSpeed = 20;
    [SyncVar]  public float Amplitude = 1;
    public float Wavelength = 2f;
    [SyncVar]  public Color color = Color.white;
    [SyncVar]  public Transform centerOfGravity;
    [SyncVar]  private bool forward = true;
    public AudioClip roll;
    public bool isOnline;

    // Update is called once per frame
    void Update() {
        if (TerrainGenerator.instance.shape == Shape.Plane) {
            if (transform.position.x < TerrainGenerator.boundary && forward) {
		        transform.Translate (new Vector3 (Time.deltaTime * speed, 0, 0));
		    } else if (forward) {
			    forward = false;
			    if (Amplitude < 1f) {
				    Destroy (this.gameObject);
			    }
			    //Amplitude = Amplitude / 4;
		    } else if (!forward && transform.position.x > -TerrainGenerator.boundary) {
                transform.Translate(new Vector3(Time.deltaTime * speed, 0, 0));
                GameObject Pulse = Instantiate(WaveGenerator.instance.antiPulse, transform.position, Quaternion.identity);
                Pulse.GetComponent<AntiPulseMove>().color = color;
                Pulse.GetComponent<AntiPulseMove>().Amplitude = Amplitude / 2;
                Pulse.GetComponent<AntiPulseMove>().speed = speed / 2;
                Destroy(this.gameObject);
            } else if (!forward) {
			    forward = true;
			    if (Amplitude < 1f) {
				    Destroy (this.gameObject);
			    }
			    //Amplitude = Amplitude / 4;
            }
            //this.color = Color.Lerp(color, Color.white, Amplitude);
        } else if (TerrainGenerator.instance.shape == Shape.Sphere) {
            transform.RotateAround(centerOfGravity.position, new Vector3(0, 0, 1), angularSpeed * Time.deltaTime);
        }
        setPositions();
        Vector3 size = transform.localScale;
        size.x = Wavelength;
        size.y = Wavelength;
    }

    void setPositions() {
        Collider2D[] hitSquares = Physics2D.OverlapCircleAll(transform.position, Wavelength, 1 << 8);
        foreach (Collider2D square in hitSquares) {
            square.GetComponent<SquareBehavior>().getPosition(Amplitude, speed, Wavelength, transform.position);
        }
    }

    bool first;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<SquareBehavior>() != null)
        {
            if (!other.gameObject.GetComponent<SquareBehavior>().firstBlock)
            {
                //audioManager.instance.Play(roll, 0.25f);
            }
            other.gameObject.GetComponent<SquareBehavior>().firstBlock = true;
            other.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
