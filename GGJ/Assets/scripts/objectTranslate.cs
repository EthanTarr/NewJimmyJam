using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectTranslate : MonoBehaviour {

    public Vector2 direction;
    public float speed;

	// Update is called once per frame
	void Update () {
        transform.Translate(direction * speed);
	}
}
