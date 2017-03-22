using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugShootCieling : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
        StartCoroutine("shootPlayer");
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    IEnumerator shootPlayer() {
        while(1 == 1) {
            GameObject testDummy = Instantiate(player, transform.position, transform.rotation);
            testDummy.GetComponent<Rigidbody2D>().isKinematic = false;
            testDummy.GetComponent<Rigidbody2D>().velocity = Vector2.down * 50;
            //testDummy.GetComponent<playertest>().smashing = true;
            yield return new WaitForSeconds(1f);
        }
    }
}
