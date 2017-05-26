using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class astroid : MonoBehaviour {

	void Start () {
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(-10, 10));
    }

    void OnTriggerEnter2D(Collider2D collision) {
        Destroy(collision.gameObject);
        Destroy(this.gameObject);
    }
}
