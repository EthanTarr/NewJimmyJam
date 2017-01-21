using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject Square;
	public int FloorSpawns = 20;
    public static GameManager instance;
	public static float boundary;

	// Use this for initialization
	void Start () {
        instance = this;
		SpawnSqures ();
		boundary = FloorSpawns * .5f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnSqures() {
		for (int i = -FloorSpawns; i < FloorSpawns; i++) {
			//Instantiate (Square, new Vector3 (.5f * i, -Mathf.Abs(Mathf.Pow(.08f *i,2)), 0), Quaternion.identity);
            Instantiate(Square, new Vector3(.5f * i, 0), Quaternion.identity);
        }
	}
}
