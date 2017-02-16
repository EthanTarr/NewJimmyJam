using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class endingUI : MonoBehaviour {

    public Animator cameraAnim;
    public static endingUI instance;
    public AudioClip ding;

    public Text[] ps;
    public GameObject anyKey;
    bool inputallowed;
    public int levelNum = 2;

    public GameObject winParticles;

    void Start() {
        instance = this;
        ps[0].gameObject.SetActive(false);
    }

    void Update() {
        if (inputallowed && Input.anyKeyDown) {
            if (scoreCard.instance.highestScore() >= scoreCard.instance.gamesToWin) {
                Application.LoadLevel(0);
                Destroy(scoreCard.instance.gameObject);
            } else {
                print(scoreCard.instance.highestScore());
                Application.LoadLevel(levelNum);
            }
        }
    }

    public void checkPlayersLeft() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 1) {
            StartCoroutine(ending(players[0].GetComponent<playertest>().playerNum));
            players[0].GetComponent<playertest>().enabled = false;
        }       
    }

    public Transform particlePosition;

    IEnumerator ending(int playerId) {
        yield return new WaitForSeconds(0.5f);
        cameraAnim.Play("endingTransition");
        yield return new WaitForSeconds(1);

        ps[0].gameObject.SetActive(true);

        for (int i = 0; i < scoreCard.instance.numOfPlayers; i++)
        {
            ps[i].text = "" + scoreCard.instance.playerScores[i];

        }
        yield return new WaitForSeconds(0.5f);
        scoreCard.instance.playerScores[playerId]++;
        audioManager.instance.Play(ding, 0.5f, 1);
        inputallowed = true;
        for (int i = 0; i < scoreCard.instance.numOfPlayers;i++) {
            ps[i].text = "" + scoreCard.instance.playerScores[i];
        }

        if (scoreCard.instance.highestScore() >= scoreCard.instance.gamesToWin) {
            Instantiate(winParticles, particlePosition.position, particlePosition.rotation);
        }

        yield return new WaitForSeconds(1f);
        anyKey.SetActive(true);


    }
}
