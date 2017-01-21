using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shockWaveDestroy : MonoBehaviour {

    public float size;

    void Start() {
        StartCoroutine(expand(size));
    }

    IEnumerator expand(float scale) {
        while (transform.localScale.x < scale) {
            transform.localScale = new Vector2(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f);
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, GetComponent<SpriteRenderer>().color.a - 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
        destroySelf();
    }

    void destroySelf() {
        Destroy(this.gameObject);
    }
}
