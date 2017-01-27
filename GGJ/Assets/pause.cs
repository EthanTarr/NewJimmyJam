using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour {

    public GameObject Pause;
    public GameObject settings;
    public AudioClip pauseBeep;
    public static pause instance;

    void Start() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Time.timeScale > 0) {
                Pause.SetActive(true);
                Time.timeScale = 0;
            } else {
                Pause.SetActive(false);
                settings.SetActive(false);
                Time.timeScale = 1;

            }

            audioManager.instance.Play(pauseBeep, 0.25f, 1);
        }
    }

    public void gotoMenu(){
        Pause.SetActive(false);
        Time.timeScale = 1;
        Application.LoadLevel(0);
    }

    public void quit() {
        Application.Quit();
    }

    public void toggleSettings() {
            settings.SetActive(!settings.activeSelf);
            Pause.SetActive(!Pause.activeSelf);
    }
}
