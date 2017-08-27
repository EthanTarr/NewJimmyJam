using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

	public GameObject Square;
	public int FloorSpawns = 20;
	public float minFloorPlacement = -20;
	public float maxFloorPlacement = 20;
    public static WaveManager instance;
	public static float boundary;

    [Space()]
    public Material mat;
    public float yScale;
    public float xOffset;

	// Use this for initialization
	void Start () {
        instance = this;
		//SpawnSqures ();
		boundary = FloorSpawns * .5f;
        Physics.IgnoreLayerCollision(6, 6, true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnSqures() {
		float width = Square.transform.localScale.x;
		float spaceToFill = Mathf.Abs(minFloorPlacement) + Mathf.Abs(maxFloorPlacement) / width;
        int index = UnityEngine.Random.Range(1, 6);

        if (GameManager.instance.totalScores() == 0) {
            index = 1;
        }

        for (float i = -spaceToFill / 2; i < spaceToFill / 2f; i++) {
            //Instantiate(Square, new Vector3(width * i, transform.position.y - 0.5f - 10/Mathf.Abs(i), 0), Quaternion.identity);
            chooseMap(index, width, i);
        }
    }

    void chooseMap(int index, float width, float i) {
        GameObject piece = null;
        switch (index) {
            case 1: piece = Instantiate(Square, new Vector3(width * i, transform.position.y, 0), Quaternion.identity); 
                break;
            case 2: piece = Instantiate(Square, new Vector3(width * i, transform.position.y  - 0.50f + Mathf.Sin(i / 10), 0), Quaternion.identity); 
                break;
            case 3: piece = Instantiate(Square, new Vector3(width * i, transform.position.y - 1 + Mathf.Abs(i / 15), 0), Quaternion.identity); 
                break;
            case 4: piece = Instantiate(Square, new Vector3(width * i, transform.position.y - Mathf.Abs(Mathf.Pow(.03f * i, 2)), 0), Quaternion.identity); 
                break;
            case 5: piece = Instantiate(Square, new Vector3(width * i, transform.position.y - 0.75f - Mathf.Sin(i / 10), 0), Quaternion.identity); 
                break;
        }

        piece.transform.parent = this.transform;
        //piece.GetComponentInChildren<MeshRenderer>().material = mat;

        float spaceToFill = Mathf.Abs(minFloorPlacement) + Mathf.Abs(maxFloorPlacement) / width;

        //piece.GetComponentInChildren<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(1f / spaceToFill, yScale));
        //piece.GetComponentInChildren<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(xOffset + i * 1f / spaceToFill, 0));
    }
}
