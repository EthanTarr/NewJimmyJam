using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killCieling : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.GetComponent<playertest>() != null) {
            Destroy(other.gameObject);
        }
    }
}
