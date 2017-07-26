using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class windFlag : MonoBehaviour {

    public float minScale, maxScale;
    public float dampen;

    public float baseY;

	void Update () {
        float yStretch = 1;
        //yStretch = (transform.position.y - baseY)/dampen;
        //yStretch = Mathf.Clamp(yStretch, minScale, maxScale);

        changeYScale(yStretch * Mathf.Sign(windZoneManager.instance.windDirection.x));
	}

    void changeYScale(float scale) {
        Vector2 curScale = transform.localScale;
        curScale.y = scale;
        transform.localScale = curScale;
    }
}
