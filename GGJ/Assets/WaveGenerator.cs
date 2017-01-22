﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour {
	public GameObject pulse;
	public GameObject antiPulse;
    public static WaveGenerator instance;

    public Color fadeIn;

	// Use this for initialization
	void Start () {
        instance = this;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("t")) {
			makeWave ();
		}
	}


	void makeWave() {
        makeWave(0, 0, fadeIn, 5);
	}

    public void StartWave(){
        makeWave(0, 0, fadeIn, 5);
    }

    public void makeWave(float position, float amplitude, Color color, float velocity) {
		GameObject Pulse = Instantiate (pulse, new Vector3 (position, transform.position.y, 0), Quaternion.identity);
		Pulse.GetComponent<PulseMove> ().Amplitude = amplitude;
        Pulse.GetComponent<PulseMove>().color = color;
        Pulse.GetComponent<PulseMove>().speed = velocity;
        GameObject AntiPulse = Instantiate (antiPulse, new Vector3(position, transform.position.y, 0), Quaternion.identity);
		AntiPulse.GetComponent<AntiPulseMove> ().Amplitude = amplitude;
        AntiPulse.GetComponent<AntiPulseMove>().color = color;
        AntiPulse.GetComponent<AntiPulseMove>().speed = velocity;
    }
}



