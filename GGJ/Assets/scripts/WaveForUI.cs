using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveForUI : MonoBehaviour {

	public GameObject Square;
	public float minFloorPlacement;
	public float maxFloorPlacement;

    public float yPos = 5;

	// Use this for initialization
	void Awake () {
		generateUISquares ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void generateUISquares() {
		float width = Square.transform.localScale.x;
		float spaceToFill = Mathf.Abs(minFloorPlacement) + Mathf.Abs(maxFloorPlacement) / width;
		for (float i = -spaceToFill / 2; i < spaceToFill / 2f; i++) {
			//Instantiate (Square, new Vector3 (.5f * i, -Mathf.Abs(Mathf.Pow(.08f *i,2)), 0), Quaternion.identity);
			GameObject piece = Instantiate (Square, new Vector3 (width * i, yPos, 0), Quaternion.identity);
            piece.transform.parent = GameObject.Find("TitleSquares").transform;
			//piece.transform.localScale = new Vector3(piece.transform.localScale.x, piece.transform.localScale.y - (Mathf.Abs(i) * .05f), 0);
			//Color fade = piece.GetComponent<SpriteRenderer> ().color;
			//fade.a = fade.a - (Mathf.Abs (i) * .03f);
			//piece.GetComponent<SpriteRenderer> ().color = fade;
		}
	}
}
