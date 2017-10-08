using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slideRight : MonoBehaviour {

    public float speed = 1;
    public float size = -12f;

    void Update() {
        transform.Translate(0.01f * speed,0,0);

        if(transform.position.x > 11.1f) {
            transform.position = new Vector2(size, transform.position.y);
        }
    }
}
