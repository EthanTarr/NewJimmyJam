using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horseMid : MonoBehaviour {

    public Transform front;
    public Transform back;
    public float yOffset;
    public float xOffset;

	void Update () {
        Vector2 halfwayPoint = front.position - xOffset * ((front.position - back.position) / 3);
        transform.position = halfwayPoint + Vector2.up * yOffset;
	}
}
