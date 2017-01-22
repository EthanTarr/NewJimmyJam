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
        int index = UnityEngine.Random.Range(1, 6);

        if (scoreCard.instance.totalScores() == 0) {
            index = 1;
        }

        for (float i = -spaceToFill / 2; i < spaceToFill / 2f; i++) {
            //Instantiate(Square, new Vector3(width * i, transform.position.y - 0.5f - 10/Mathf.Abs(i), 0), Quaternion.identity);
            chooseMap(index, width, i);
        }
    }

    void chooseMap(int index, float width, float i) {
        switch (index) {
            case 1: Instantiate(Square, new Vector3(width * i, transform.position.y, 0), Quaternion.identity); break;
            case 2: Instantiate(Square, new Vector3(width * i, transform.position.y  + Mathf.Sin(i / 10), 0), Quaternion.identity); break;
            case 4: Instantiate(Square, new Vector3(width * i, transform.position.y - 1 + Mathf.Abs(i / 15), 0), Quaternion.identity); break;
            case 5: Instantiate(Square, new Vector3(width * i, transform.position.y - Mathf.Abs(Mathf.Pow(.03f * i, 2)), 0), Quaternion.identity); break;
            case 6: Instantiate(Square, new Vector3(width * i, transform.position.y - 0.75f - Mathf.Sin(i / 10), 0), Quaternion.identity); break;
        }
    }
}
