using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour {

    public static settings instance;
    [Range(0, 1)] public float volume;
    [Range(0, 1)] public float fx;
    private AudioSource musicAudio;

    public Slider musicVol;
    public Slider fxVol;

    void Start() {
        instance = this;
        musicAudio = GetComponent<AudioSource>();
    }

    public void changeMusicVol() {
        musicAudio.volume = musicVol.value;
    }

    public void changeFXVol() {
        fx = fxVol.value;
    }
}
