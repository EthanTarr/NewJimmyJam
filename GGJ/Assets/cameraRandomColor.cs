using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRandomColor : MonoBehaviour {

    public Color[] colors;

    void Start() {
        GetComponent<Camera>().backgroundColor = colors[Random.Range(0, colors.Length - 1)];
    }

    public void contactWaveGenerator() {
        WaveGenerator.instance.StartWave();
    }

    public void activatePlayers() {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
