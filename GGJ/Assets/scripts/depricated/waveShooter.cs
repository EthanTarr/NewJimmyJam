using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveShooter : MonoBehaviour {

    public GameObject pulse;
    public Color pColor;
    public float amplitude;
    public float speed;
    

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GameObject wave = Instantiate(pulse, transform.position, transform.rotation);
            wave.GetComponent<AntiPulseMove>().speed = speed;
            wave.GetComponent<AntiPulseMove>().Amplitude = amplitude;
            wave.GetComponent<AntiPulseMove>().color = pColor;
        }
    }
}
