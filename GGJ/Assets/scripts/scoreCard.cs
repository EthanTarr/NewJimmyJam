using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreCard : MonoBehaviour {

    public static scoreCard instance;
    public int[] playerScores;
    public int numOfPlayers = 2;
	public bool ConeHeadMode = false;
    public int gamesToWin;
    private UnityEngine.UI.Toggle ConeHeadToggle;

    void Awake() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
			Destroy(this.gameObject);
        }
        playerScores = new int[numOfPlayers];
        if (ConeHeadToggle == null)
        {
            //ConeHeadToggle = GameObject.Find("ConeHeadToggle").GetComponent<UnityEngine.UI.Toggle>();
           // Debug.Log("1" + ConeHeadToggle);
            //GameObject.Find("GameOptions").active = false;
        }
    }

    private void Start() {
        ConeHeadToggle = GameObject.Find("ConeHeadToggle").GetComponent<UnityEngine.UI.Toggle>();
        scoreCard.instance.isConeHeadMode();
        GameObject.Find("GameOptions").active = false;
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
        Debug.Log("2" + ConeHeadToggle);
        ConeHeadMode = ConeHeadToggle.isOn;
	}

    public void maxGames() {
        if (int.TryParse(GameObject.Find("GameCounter").GetComponent<UnityEngine.UI.InputField>().text, out gamesToWin))
            int.TryParse(GameObject.Find("GCPlaceholder").GetComponent<UnityEngine.UI.Text>().text, out gamesToWin);
    }

    public void clearScore() {
        for (int j = 0; j < numOfPlayers; j++) {
            playerScores[j] = 0;
        }
    }
}
