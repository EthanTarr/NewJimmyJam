using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehavior : MonoBehaviour {

	//public int Amplitude = 2;
	public float Wavelength = 2f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		float FinalYPos = 0;
		foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("Pulse")) {
			float xPos = transform.position.x;
			float xPulsePos = pulse.transform.position.x;

			if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength) {
				FinalYPos += pulse.GetComponent<PulseMove>().Amplitude * Mathf.Sin (((Mathf.PI / Wavelength) * (xPos - xPulsePos)));
			}
		}
		foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("AntiPulse")) {
			float xPos = transform.position.x;
			float xPulsePos = pulse.transform.position.x;

			if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength) {
				FinalYPos += -pulse.GetComponent<AntiPulseMove>().Amplitude * Mathf.Sin ((Mathf.PI / Wavelength) * (xPos - xPulsePos));
			}

		}
		transform.position = new Vector3 (transform.position.x, FinalYPos, 0);
	}
}
