using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject Square;
	public int FloorSpawns = 20;

	// Use this for initialization
	void Start () {
		SpawnSqures ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnSqures() {
		for (int i = -FloorSpawns; i < FloorSpawns; i++) {
			Instantiate (Square, new Vector3 (.5f * i, 0, 0), Quaternion.identity);
		}
	}
}
