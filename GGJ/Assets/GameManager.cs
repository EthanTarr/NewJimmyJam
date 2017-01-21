using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject Square;
	public int FloorSpawns = 20;
	public float minFloorPlacement = -20;
	public float maxFloorPlacement = 20;
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
		float width = Square.transform.localScale.x;
		float spaceToFill = Mathf.Abs(minFloorPlacement) + Mathf.Abs(maxFloorPlacement) / width;
		for (float i = -spaceToFill / 2; i < spaceToFill / 2f; i++) {
			//Instantiate (Square, new Vector3 (.5f * i, -Mathf.Abs(Mathf.Pow(.08f *i,2)), 0), Quaternion.identity);
			Instantiate (Square, new Vector3 (width * i, 0, 0), Quaternion.identity);
		}
	}
}
