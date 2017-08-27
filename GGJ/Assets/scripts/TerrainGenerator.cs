using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainGenerator : MonoBehaviour {

    private float radius;
    public float SquareWidth = 2f;
    public Material squareMaterial;
    public GameObject Square;
    private float length;
    public GameObject Spike;
    public bool invertSpikes = false;

    [HideInInspector] public int mapIndex;

    //Later on this might not work if we end up using 
    //Terrain Generator to do background/enviornmental Objects
    public static TerrainGenerator instance; 

    [SerializeField]
    public Shape shape = Shape.Plane;

    [SerializeField]
    public Platform plat = Platform.Square;

    // Use this for initialization
    void Start () {
        instance = this;
        Generate();
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void Generate() {
        length = this.gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        radius = this.gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
        Square.transform.localScale = new Vector3(SquareWidth, shape == Shape.Plane ? 10 : (radius / 2), 1);

        ArrayList temp = new ArrayList();
        foreach (Transform tran in this.gameObject.GetComponentInChildren<Transform>()) {
            temp.Add(tran);
        }
        for(int i = 0; i < temp.Count; i++) {
            DestroyImmediate(((Transform)temp[i]).gameObject);
        }
        if (plat == Platform.Square)
        {
            if (shape == Shape.Plane)
            {
                generatePlatform();
            }
            else if (shape == Shape.Sphere)
            {
                generateCircle();
            }
        }
        else if (plat == Platform.Spike)
        {
            if (shape == Shape.Plane)
            {
                generateSpikePlatform();
            }
            else if (shape == Shape.Sphere)
            {
                generateCircleSpikes();
            }
        }
    }

    void generateCircle() {
        for (float i = 0; i < (2 * Mathf.PI); i+= (2 * SquareWidth / (Mathf.PI * radius))) {
            GameObject child = Instantiate(Square, new Vector3(Mathf.Cos(i) * radius, Mathf.Sin(i) * radius, 0) + transform.position, Quaternion.Euler(new Vector3(0, 0, (Mathf.Rad2Deg * i) + 90)));
            child.GetComponent<SquareBehavior>().CenterOfGravity = transform.position;
            child.GetComponentInChildren<MeshRenderer>().material = squareMaterial;
            child.transform.parent = this.gameObject.transform;
        }
    }

    void generateCircleSpikes() {
        int rotate = 90;
        if (invertSpikes) {
            rotate = -90;
        }
        for (float i = 0; i < (2 * Mathf.PI); i += (2.5f * Spike.transform.localScale.x / (Mathf.PI * radius))) {
            GameObject child = Instantiate(Spike, new Vector3(Mathf.Cos(i) * radius, Mathf.Sin(i) * radius, 0) + transform.position, Quaternion.Euler(new Vector3(0, 0, (Mathf.Rad2Deg * i + rotate))));
            child.transform.parent = this.gameObject.transform;
        }
    }

    void generatePlatform() {
        float spaceToFill = length / SquareWidth;
        mapIndex = 1;

        if(Application.isPlaying)
            mapIndex = GameManager.instance.totalScores() == 0 ? 1 : UnityEngine.Random.Range(1, 6);

        for (float i = -spaceToFill / 2; i < spaceToFill / 2f; i++) {
            GameObject child = Instantiate(Square, new Vector3((SquareWidth * i) + transform.position.x, customPlatformPos(mapIndex, i), 0), Quaternion.identity);
            child.transform.parent = this.gameObject.transform;
            child.GetComponentInChildren<MeshRenderer>().material = squareMaterial;
        }
    }

     float customPlatformPos(int mapIndex, float floorIndex) {
        switch (mapIndex) {
            case 1:
                return transform.position.y;
            case 2:
                return transform.position.y - 0.50f + Mathf.Sin(floorIndex / 10);
            case 3:
                return transform.position.y - 1 + Mathf.Abs(floorIndex / 15);
            case 4:
                return transform.position.y - Mathf.Abs(Mathf.Pow(.03f * floorIndex, 2));
            case 5:
                return transform.position.y - 0.75f - Mathf.Sin(floorIndex / 10);
        }
        return 0;
    }

    void generateSpikePlatform() {
        float spaceToFill = length / (Spike.transform.localScale.x * .85f);
        int rotate = 0;
        if (invertSpikes) {
            rotate = 180;
        }
        for (float i = -spaceToFill / 2f; i < spaceToFill / 2f; i++) {
            GameObject child = Instantiate(Spike, new Vector3(((Spike.transform.localScale.x * .85f) * i) + transform.position.x, transform.position.y, 0), Quaternion.Euler(0,0,rotate));
            child.transform.parent = this.gameObject.transform;
        }
    }
}

[SerializeField]
public enum Shape {
    Plane = 0,
    Sphere = 1,
}

[SerializeField]
public enum Platform { 
    Square = 0,
    Spike = 1,
}