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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0)
            {
                text.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                text.SetActive(false);
                Time.timeScale = 1;

            }

            audioManager.instance.Play(pauseBeep, 0.25f, 1);
        }
    }

    public void gotoMenu(){
        text.SetActive(false);
        Time.timeScale = 1;
        Application.LoadLevel(0);
    }

    public void quit() {
        Application.Quit();
    }
}
