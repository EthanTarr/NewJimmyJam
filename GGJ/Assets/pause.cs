using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pause : MonoBehaviour {

    public GameObject text;
    public AudioClip pauseBeep; 

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (Time.timeScale > 0) {
                GetComponent<Image>().enabled = true;
                text.SetActive(true);
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
                GetComponent<Image>().enabled = false;
                text.SetActive(false);
            }

            audioManager.instance.Play(pauseBeep, 0.25f, 1);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 0) {
            Application.Quit();
        }
    }
}
