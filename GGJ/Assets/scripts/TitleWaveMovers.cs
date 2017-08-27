using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleWaveMovers : MonoBehaviour {

    float originalYPos;
    float yPos;

    public bool backWave;

	// Use this for initialization
	void Start () {
        if (!backWave)
            GetComponent<TextMesh>().color = ButtonActions.instance.textColor;
        else
            GetComponent<SpriteRenderer>().color = ButtonActions.instance.waveColor;

        originalYPos = transform.position.y;
        yPos = originalYPos;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (!backWave)
           yPos = originalYPos + Mathf.Sin(Time.time * (transform.position.x + 10) / 2) / 8;
        else {
            yPos = originalYPos + Mathf.Sin(Time.time * (transform.position.x + 10) / 8) / 4;
        }

        transform.position = new Vector3 (transform.position.x, yPos, transform.position.z);
	}
}
