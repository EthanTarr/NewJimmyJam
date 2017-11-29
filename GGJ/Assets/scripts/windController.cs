using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windController : MonoBehaviour {

    [HideInInspector] public Vector2 windDirection;
    public float speed;

    private void OnTriggerEnter2D(Collider2D collision) {
        
    }

    private void OnTriggerStay2D(Collider2D collision) {

        if (collision.GetComponent<playerController>() != null) {
            Vector2 newDir = collision.transform.position;
            newDir += windDirection * speed;
            collision.transform.position = newDir;
        }

        if (collision.gameObject.name == "snowman") {
            Vector3 newDir = collision.transform.position;
            newDir += (Vector3)windDirection * speed / 3;
            collision.transform.position = newDir;
        }
    }
}
