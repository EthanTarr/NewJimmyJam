using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour {

    public GameObject Pause;
    public GameObject settings;
    public AudioClip pauseBeep;
    public static pause instance;
    [Space()]
    public GameObject firstButton;
    public GameObject firstSettingsButton;

    void Update() {
        if (Input.GetButtonDown("Pause")) {
            togglePause();
        }
    }

    public void togglePause() {
        if (Pause.activeSelf || settings.activeSelf) {
            Pause.SetActive(false);
            settings.SetActive(false);
            Time.timeScale = 1;
        } else {
            Pause.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstButton);
            //EventSystem.current.gameObject.GetComponent<myInputModule>().submitButton = "Submit";
            Time.timeScale = 0;
        }
        audioManager.instance.Play(pauseBeep, 0.25f, 1);
    }

    public void gotoMenu(){
        Pause.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void quit() {
        Application.Quit();
    }

    public void toggleSettings() {
        settings.SetActive(!settings.activeSelf);
        Pause.SetActive(!Pause.activeSelf);
        if (settings.activeSelf) {
            EventSystem.current.SetSelectedGameObject(firstSettingsButton);
        } else {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }
}
