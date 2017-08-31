using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killCieling : MonoBehaviour {

    GameObject target;
    Vector2 offset;

    private void Start() {
        if (!GameManager.instance.instantBounceKill || Application.loadedLevelName == "Controller Setup") {
            Destroy(this.gameObject);
        }

        target = transform.parent.gameObject.transform.parent.gameObject;
        offset = transform.position - target.transform.position;
        transform.parent = null;
    }

    private void LateUpdate() {
        if (target == null)
        {
            Destroy(this.gameObject);
        }
        transform.position = target.transform.position + (Vector3)offset;

    }
}
