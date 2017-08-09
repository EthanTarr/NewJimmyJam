using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehavior : MonoBehaviour {

	public float TotalAmplitude;
	public float Wavelength = 2f;
	public float FloorOscillation = .02f;
    public float OscillationSpeed = 0.01f;
	[HideInInspector] public float initialY = 0;
    [HideInInspector] public float initialX = 0;
    private float standardY;
    private float standardX;
    [HideInInspector] public bool firstBlock;
    [HideInInspector] public Vector3 CenterOfGravity;

    Renderer squareMaterial;
    public Color matColor;
    public Color ampColor;

    Vector2 lastPosition;
	void Start () {
        lastPosition = transform.position;
        standardY = transform.position.y;
        standardX = transform.position.x;
       

        squareMaterial = transform.GetChild(0).GetComponent<Renderer>();
        StartCoroutine(physicsCheck());
    }

    float maxAmplitude = 5f;

    public float dampen = 1;

    void getPosition() {
        initialY = transform.position.y;
        initialX = transform.position.x;
        TotalAmplitude = 0;
        standardY += FloorOscillation * (Mathf.Sin(Time.time * OscillationSpeed));
        standardX += FloorOscillation * (Mathf.Sin(Time.time * OscillationSpeed));

        foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("Pulse"))
        {
            float xPos = transform.position.x;
            float xPulsePos = pulse.transform.position.x;

            if ((transform.position - pulse.transform.position).x < Wavelength && (transform.position - pulse.transform.position).x > -Wavelength) { //when working with sphere switch .x to .magnitude
                TotalAmplitude += pulse.GetComponent<PulseMove>().Amplitude * (pulse.GetComponent<PulseMove>().speed / 4) * Mathf.Sin(((Mathf.PI / Wavelength) * (xPos - xPulsePos)));
            }
        }
        foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("AntiPulse"))
        {
            float xPos = transform.position.x;
            float xPulsePos = pulse.transform.position.x;

            if ((transform.position - pulse.transform.position).x < Wavelength && (transform.position - pulse.transform.position).x > -Wavelength) { //when working with sphere switch .x to .magnitude
                TotalAmplitude += -pulse.GetComponent<AntiPulseMove>().Amplitude * (pulse.GetComponent<AntiPulseMove>().speed / 4) * Mathf.Sin((Mathf.PI / Wavelength) * (xPos - xPulsePos));
            }
        }
        TotalAmplitude = Mathf.Clamp(TotalAmplitude, -15, 15);
        Vector3 vector = (-((-transform.position + CenterOfGravity).normalized)) * TotalAmplitude;

        if (TerrainGenerator.instance != null && TerrainGenerator.instance.shape == Shape.Sphere) {
            transform.position = new Vector3(Mathf.Lerp(initialX, standardX + vector.x, Time.deltaTime), Mathf.Lerp(initialY, standardY + vector.y, Time.deltaTime), 0);
        } else {
            transform.position = new Vector3(initialX, Mathf.Lerp(initialY, standardY + vector.y, Time.deltaTime), transform.position.z);
        }


        getVelocity();

        if (firstBlock) {
            //GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, floorColor, Time.deltaTime);
        }
    }

    [HideInInspector] public float velocity;
    void getVelocity() {
        velocity =  transform.position.y - lastPosition.y;
        lastPosition = transform.position;
    }

    IEnumerator physicsCheck(){
        while(1 == 1) {
            getPosition();
            squareMaterial.material.SetColor("_Color", Color.Lerp(matColor, ampColor, Mathf.Abs(TotalAmplitude) / 15));
            //squareMaterial.material.SetColor("_EmmissionColor", matColor);
            yield return new WaitForSeconds(0.01f);
        }
    }
}