using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    private float radius;
    public float SquareWidth = 2f;
    public GameObject Square;
    private float length;
    public GameObject Spike;
    public bool invertSpikes = false;
    public Shape shape = Shape.Plane;
    public Platform plat = Platform.Square;

    // Use this for initialization
    void Start () {
        Generate();
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void Generate() {
        length = this.gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        radius = this.gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
        Square.transform.localScale = new Vector3(SquareWidth, radius / 2, 1);

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
        for (float i = -spaceToFill / 2; i < spaceToFill / 2f; i++) {
            GameObject child = Instantiate(Square, new Vector3((SquareWidth * i) + transform.position.x, transform.position.y, 0), Quaternion.identity);
            child.transform.parent = this.gameObject.transform;
        }
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

    public enum Shape {
        Plane = 0,
        Sphere = 1,
    }

    public enum Platform {
        Square = 0,
        Spike = 1,
    }
}
