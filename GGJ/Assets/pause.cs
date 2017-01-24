using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour {

    public GameObject text;
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


    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (Time.timeScale > 0) {
                text.SetActive(true);
                Time.timeScale = 0;
            } else {
                text.SetActive(false);
                Time.timeScale = 1;
                
            }

            audioManager.instance.Play(pauseBeep, 0.25f, 1);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 0) {
            Application.Quit();
        }
    }
}
