using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class configureSoundSettings : MonoBehaviour {

    public Slider musicVol;
    public Slider fxVol;

    private void Start() {
        musicVol.value = settings.instance.musicAudio.volume;
        fxVol.value = settings.instance.fx;
    }

    public void changeMusicVol() {
        settings.instance.musicAudio.volume = musicVol.value;
    }

    public void changeFXVol()
    {
        settings.instance.fx = fxVol.value;
    }
}
