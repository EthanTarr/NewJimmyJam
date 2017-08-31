using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainGenerator : MonoBehaviour {

    private float radius;
    [HideInInspector] public float SquareWidth = 2f;
    [HideInInspector] public Material squareMaterial;
    [HideInInspector] public GameObject Square;
    private float length;
    [HideInInspector] public GameObject Spike;
    [HideInInspector] public bool invertSpikes = false;

    public GameObject testSquare;
    public int FloorSpawns = 20;
    public static float boundary;

    [Header("Terrain Shape")]
    public bool mapDebug;
    public int mapIndex;

    //Later on this might not work if we end up using 
    //Terrain Generator to do background/enviornmental Objects
    public static TerrainGenerator instance; 

    [SerializeField][HideInInspector]
    public Shape shape = Shape.Plane;

    [SerializeField][HideInInspector]
    public Platform plat = Platform.Square;

    void Start () {
        instance = this;
        boundary = FloorSpawns * 0.5f;
        Generate();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            WaveGenerator.instance.makeWave(testSquare.transform.position, 10, Color.red, 1, this.transform);
        }
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
        

        if (Application.isPlaying)
            if (!mapDebug) {
                mapIndex = 1;
                mapIndex = GameManager.instance.totalScores() == 0 ? 1 : UnityEngine.Random.Range(1, 10);
            }

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
            case 6:
                return transform.position.y - 0.5f - floorIndex * 0.025f;
            case 7:
                return transform.position.y + floorIndex * 0.025f;
            case 8:
                return transform.position.y - 0.75f - Mathf.Sin((floorIndex + 20) / 10);
            case 9:
                return transform.position.y - 0.75f - Mathf.Sin((floorIndex - 15) / 10);
            case 10:
                return transform.position.y + 0.5f - Mathf.Abs(floorIndex / 25);
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