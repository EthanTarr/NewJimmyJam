using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour {

    public static settings instance;
    [Range(0, 1)] public float volume;
    [Range(0, 1)] public float fx;
    [HideInInspector] public AudioSource musicAudio;

    void Start() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }

        musicAudio = GetComponent<AudioSource>();
    }
}
