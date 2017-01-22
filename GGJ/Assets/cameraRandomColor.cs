using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRandomColor : MonoBehaviour {

    public Color[] colors;

    void Start() {
        GetComponent<Camera>().backgroundColor = colors[Random.Range(0, colors.Length - 1)];
    }
}
