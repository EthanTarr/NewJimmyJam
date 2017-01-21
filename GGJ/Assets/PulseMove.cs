using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseMove : MonoBehaviour {

	public float speed = 5;
	public float Amplitude = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (Time.deltaTime * speed, 0, 0));
	}
}
