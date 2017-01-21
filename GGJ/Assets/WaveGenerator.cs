﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour {
	public GameObject pulse;
	public GameObject antiPulse;
    public static WaveGenerator instance;

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
		Instantiate (pulse, new Vector3 (0, transform.position.y, 0), Quaternion.identity);
		Instantiate (antiPulse, new Vector3(0, transform.position.y, 0), Quaternion.identity);
	}

	public void makeWave(float position, float amplitude, Color color) {
		GameObject Pulse = Instantiate (pulse, new Vector3 (position, transform.position.y, 0), Quaternion.identity);
		Pulse.GetComponent<PulseMove> ().Amplitude = amplitude;
        Pulse.GetComponent<PulseMove>().color = color;
        GameObject AntiPulse = Instantiate (antiPulse, new Vector3(position, transform.position.y, 0), Quaternion.identity);
		AntiPulse.GetComponent<AntiPulseMove> ().Amplitude = amplitude;
        AntiPulse.GetComponent<AntiPulseMove>().color = color;
    }
}



