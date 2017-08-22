using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShadow : MonoBehaviour {

    public SpriteRenderer parentSpriteRender;

	// Use this for initialization
	void Start () {
        StartCoroutine("updateShadow");
	}


    IEnumerator updateShadow() {
        while (1 == 1) {
            GetComponent<SpriteRenderer>().sprite = parentSpriteRender.sprite;
            GetComponent<SpriteRenderer>().flipX = parentSpriteRender.flipX;
            yield return new WaitForFixedUpdate();
        }
    }
}
