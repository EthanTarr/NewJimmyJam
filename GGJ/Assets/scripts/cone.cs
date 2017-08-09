using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cone : MonoBehaviour {

	void Start () {
        this.gameObject.SetActive(GameManager.instance.ConeHeadMode) ;
	}

    void Update() {
        GetComponent<SpriteRenderer>().color = transform.parent.GetComponent<SpriteRenderer>().color;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Spike") {
            print("hoi");
        }

        if (collision.gameObject.GetComponent<playerController>()) {
            collision.gameObject.GetComponent<playerController>().die();
        }
    }
}
