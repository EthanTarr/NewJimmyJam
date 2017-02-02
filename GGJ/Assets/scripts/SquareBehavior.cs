using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehavior : MonoBehaviour {

	public float TotalAmplitude;
	public float Wavelength = 2f;
	public float FloorOscillation = .02f;
    public float OscillationSpeed = 0.01f;
	private float initialY = 0;
	private float standardY;

    [HideInInspector] public bool firstBlock;

    // Use this for initialization
    Vector2 lastPosition;
	void Start () {
        lastPosition = transform.position;
		standardY = transform.position.y;
	}

    float maxAmplitude = 5f;
	// Update is called once per frame
	void Update () {
		initialY = transform.position.y;
		TotalAmplitude = 0;
		standardY += FloorOscillation * (Mathf.Sin (Time.time * OscillationSpeed));

		foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("Pulse")) {
			float xPos = transform.position.x;
			float xPulsePos = pulse.transform.position.x;

			if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength) {
				TotalAmplitude += pulse.GetComponent<PulseMove>().Amplitude * Mathf.Sin (((Mathf.PI / Wavelength) * (xPos - xPulsePos)));
            }
		}
		foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("AntiPulse")) {
            float xPos = transform.position.x;
			float xPulsePos = pulse.transform.position.x;

			if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength) {
				TotalAmplitude += -pulse.GetComponent<AntiPulseMove>().Amplitude * Mathf.Sin ((Mathf.PI / Wavelength) * (xPos - xPulsePos));
			}
        }
		TotalAmplitude = Mathf.Clamp (TotalAmplitude, -10,10);

		transform.position = new Vector3 (transform.position.x, Mathf.Lerp(initialY, TotalAmplitude + standardY, Time.deltaTime), 0);
        getVelocity();

        if (firstBlock) {
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, Color.white, Time.deltaTime);
        }

    }

    void MovePassenger(float velocity) {

        float direction = Mathf.Sign(velocity);

        if (velocity != 0) {

        }
    }

    [HideInInspector] public float velocity;
    void getVelocity() {
        velocity =  transform.position.y - lastPosition.y;
        lastPosition = transform.position;
    }
}