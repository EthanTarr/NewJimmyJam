using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windows : MonoBehaviour {

    public GameObject[] window;
    TerrainGenerator mapTerrain;

	// Use this for initialization
	void Start () {
        mapTerrain = FindObjectOfType<TerrainGenerator>();
        startThing();
	}

    void startThing() {
        for (int i = 0; i < window.Length; i++) {
            window[i].transform.position = new Vector3(window[i].transform.position.x, customPlatformPos(1, i), window[i].transform.position.z);
        }
    }

    float customPlatformPos(int mapIndex, int floorIndex)
    {
        switch (mapIndex)
        {
            case 1:
                return window[floorIndex].transform.position.y;
            case 2:
                return window[floorIndex].transform.position.y - 0.50f + Mathf.Sin(mapIndex / 10);
            case 3:
                return window[floorIndex].transform.position.y - 1 + Mathf.Abs(mapIndex / 15);
            case 4:
                return window[floorIndex].transform.position.y - Mathf.Abs(Mathf.Pow(.03f * mapIndex, 2));
            case 5:
                return (window[floorIndex].transform.position.y - 0.75f - Mathf.Sin(mapIndex / 10));
        }
        return window[mapIndex].transform.position.y;
    }
}
