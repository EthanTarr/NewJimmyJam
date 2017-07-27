using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class rotatingGears : MonoBehaviour {

    public float rotate;


    private void Update() {
        transform.Rotate(0,0,rotate);
    }
}
