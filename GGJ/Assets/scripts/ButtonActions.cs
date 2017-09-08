using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour {
    public static ButtonActions instance;

    public menuColorScheme[] colorScheme;
    public int chosenColor = 0;

    [HideInInspector] public Color backgroundColor;
    [HideInInspector] public Color waveColor;
    [HideInInspector] public Color textColor;

    // Use this for initialization
    void Awake () {
        instance = this;
        //scoreCard.instance.isConeHeadMode();

        chosenColor = Random.Range(0, colorScheme.Length);

        backgroundColor = colorScheme[chosenColor].backgroundColor;
        waveColor = colorScheme[chosenColor].waveColor;
        textColor = colorScheme[chosenColor].textColor;

        Camera.main.backgroundColor = backgroundColor;
	}

    private void Start() {
        //scoreCard.instance.gamesToWin = 1;
    }

    // Update is called once per frame
    void Update () {
    }

	public void PlayGame() {
        StartCoroutine(screenTransition.instance.fadeOut("Controller Setup"));
	}

    [System.Serializable]
    public struct menuColorScheme {
        public Color backgroundColor;
        public Color waveColor;
        public Color textColor;
    }
}
