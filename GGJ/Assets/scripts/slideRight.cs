using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slideRight : MonoBehaviour {



    void Update() {
        transform.Translate(0.01f,0,0);

        if(transform.position.x > 15.1f) {
            transform.position = new Vector2(-12f, transform.position.y);
        }
    }
}
