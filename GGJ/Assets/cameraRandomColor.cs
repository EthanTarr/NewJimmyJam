using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRandomColor : MonoBehaviour {

    public Color[] colors;
    public AudioClip go;

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

    public void playGo() {
        audioManager.instance.Play(go, 0.5f, 1);
    }
}
