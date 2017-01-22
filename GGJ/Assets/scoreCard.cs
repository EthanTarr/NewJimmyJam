using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreCard : MonoBehaviour {

    public static scoreCard instance;
    public int[] playerScores;
    public int numOfPlayers = 2;
	public static bool ConeHeadMode = false;

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
		
	public void isConeHeadMode() {
		ConeHeadMode = GameObject.Find("Toggle").GetComponent<UnityEngine.UI.Toggle>().isOn;
	}

	public void OnLevelWasLoaded(int level) {
		
		if (level == 1) {
			
			foreach (GameObject cone in GameObject.FindGameObjectsWithTag("Cone")) {
				Debug.Log (ConeHeadMode);
				cone.SetActive (ConeHeadMode);
			}
		}
	}
}
