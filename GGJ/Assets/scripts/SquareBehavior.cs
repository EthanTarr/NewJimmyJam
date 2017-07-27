﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehavior : MonoBehaviour {

	public float TotalAmplitude;
	public float Wavelength = 2f;
	public float FloorOscillation = .02f;
    public float OscillationSpeed = 0.01f;
	[HideInInspector] public float initialY = 0;
	private float standardY;
    [HideInInspector] public bool firstBlock;

    Vector2 lastPosition;
	void Start () {
        lastPosition = transform.position;
        standardY = transform.position.y;
        StartCoroutine(physicsCheck());
	}

    float maxAmplitude = 5f;

    public float dampen = 1;

	void Update () {
        //getPosition();
    }

    void getPosition() {
        initialY = transform.position.y;
        TotalAmplitude = 0;
        standardY += FloorOscillation * (Mathf.Sin(Time.time * OscillationSpeed));

        foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("Pulse"))
        {
            float xPos = transform.position.x;
            float xPulsePos = pulse.transform.position.x;

            if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength)
            {
                TotalAmplitude += pulse.GetComponent<PulseMove>().Amplitude * (pulse.GetComponent<PulseMove>().speed / 4) * Mathf.Sin(((Mathf.PI / Wavelength) * (xPos - xPulsePos)));
            }
        }
        foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("AntiPulse"))
        {
            float xPos = transform.position.x;
            float xPulsePos = pulse.transform.position.x;

            if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength)
            {
                TotalAmplitude += -pulse.GetComponent<AntiPulseMove>().Amplitude * (pulse.GetComponent<AntiPulseMove>().speed / 4) * Mathf.Sin((Mathf.PI / Wavelength) * (xPos - xPulsePos));
            }
        }
        TotalAmplitude = Mathf.Clamp(TotalAmplitude, -10, 10);
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(initialY, TotalAmplitude + standardY, Time.deltaTime / dampen), transform.position.z);
        getVelocity();

        if (firstBlock) {
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, floorColor, Time.deltaTime);
        }
    }

    public Color floorColor = Color.white;

    [HideInInspector] public float velocity;
    void getVelocity() {
        velocity =  transform.position.y - lastPosition.y;
        lastPosition = transform.position;
    }

    IEnumerator physicsCheck(){
        while(1 == 1) {
            getPosition();
            yield return new WaitForSeconds(0.01f);
        }
    }
}