using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreCard : MonoBehaviour {

    public static scoreCard instance;
    public int[] playerScores;
    public int numOfPlayers = 2;

    void Awake() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
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
}
