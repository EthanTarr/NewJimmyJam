using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class victoryScreenManager : MonoBehaviour {

    public GameObject confetti;
    public playerController winner;
    public GameObject placeUI;
    public GameObject[] spotLights;
    public GameObject[] animSpotLights;
    [Space()]
    public AudioClip drumroll;
    public AudioClip end;
    public AudioClip ding;


    void Start() {
        StartCoroutine("placementSequence");
    }

    IEnumerator placementSequence() {
        playerController[] players = playerSpawner.instance.players;

        Array.Sort(players);
        yield return new WaitForSeconds(1f);
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 4; i++) {
            yield return new WaitForSeconds(0.5f * (i + 1));
            if (i < players.Length) {
                GameObject newUI = Instantiate(placeUI, players[i].transform);
                newUI.transform.position = players[i].transform.position;
                newUI.transform.localScale = Vector3.one * 0.005542449f;
                newUI.transform.position += Vector3.up * 1.5f;
                newUI.GetComponent<TextMesh>().text = "" + (players.Length - i);
                newUI.GetComponent<TextMesh>().color = players[i].spriteAnim.GetComponent<SpriteRenderer>().color;
                if (i == players.Length - 1) {
                    winner = players[i];
                    GetComponent<AudioSource>().loop = false;
                    GetComponent<AudioSource>().clip = end;
                    GetComponent<AudioSource>().Play();
                }
                audioManager.instance.Play(ding, 1, 0.85f + 0.1f * i);
            }
        }

        yield return new WaitForSeconds(0f);
        foreach (GameObject light in spotLights) {
            light.transform.parent.transform.parent.GetComponent<Animator>().enabled = false;
        }
    }

    void Update() {
        if (winner != null) {
            confetti.transform.position = winner.transform.position + Vector3.up * 10.5f;
            foreach (GameObject light in spotLights) {
                light.transform.position = Vector3.Lerp(light.transform.position, winner.transform.position + Vector3.forward * 10, Time.deltaTime * 10);
                light.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
            }

            if (Input.GetButtonDown("Enter" + winner.playerControl)) {
                Application.LoadLevel(1);
                GameManager.instance.clearScore();
            }
        }
    }
}
