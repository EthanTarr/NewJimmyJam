using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleWaveMovers : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x, Mathf.Lerp(transform.position.y, 2.5f + Mathf.Sin(Time.time * transform.position.x), Time.deltaTime), 0);
	}
}
