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

    public GameObject winParticles;

    void Start() {
        instance = this;
        ps[0].gameObject.SetActive(false);
    }

    void Update() {
        if (inputallowed && Input.anyKeyDown) {
            if (scoreCard.instance.highestScore() >= scoreCard.instance.gamesToWin)  {
                Application.LoadLevel(0);
                Destroy(scoreCard.instance.gameObject);
            } else {
                print(scoreCard.instance.highestScore());
                Application.LoadLevel(1);
            }
        }
    }

    public void startEnd(int playerId) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players) {
            player.GetComponent<playertest>().enabled = false;       
        }
        StartCoroutine(ending(playerId));
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
