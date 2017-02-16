using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreCard : MonoBehaviour {

    public static scoreCard instance;
    public int[] playerScores;
    public int numOfPlayers = 2;
	public bool ConeHeadMode = false;
    public int gamesToWin;

    void Awake() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
			Destroy(this.gameObject);
        }
        playerScores = new int[numOfPlayers];
    }

    public int totalScores() {
        int total = 0;
        foreach (int i in playerScores) {
            total += i;
        }

        return total;
    }

    public int highestScore() {
        int max = 0;
        foreach (int i in playerScores) {
            max = Mathf.Max(max, i);
        }
        return max;
    }

	public void isConeHeadMode() {
		ConeHeadMode = GameObject.Find("Toggle").GetComponent<UnityEngine.UI.Toggle>().isOn;
	}

    public void maxGames() {
        int.TryParse(GameObject.Find("InputField").GetComponent<UnityEngine.UI.InputField>().text, out gamesToWin);
    }

    public void clearScore() {
        for (int j = 0; j < numOfPlayers; j++) {
            playerScores[j] = 0;
        }
    }
}
