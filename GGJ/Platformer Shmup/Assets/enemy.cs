using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {

    public int health;
    SpriteRenderer renderer;

    private void Start() {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void hit() {
        health--;
        if (health <= 0) {
            Destroy(this.gameObject);
        }
    }
}
