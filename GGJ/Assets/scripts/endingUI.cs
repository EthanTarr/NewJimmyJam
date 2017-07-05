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
    public float spacing;
    float width;

    [Space()]
    public GameObject scoreText;

    public GameObject winParticles;

    void Start() {
        instance = this;
        levelNum = scoreCard.instance.numOfPlayers;

        width = spacing * 4 * levelNum;
        for (int i = 0; i < levelNum - 1; i++) {
            GameObject text = Instantiate(scoreText, transform.position - Vector3.right * (width / 2 - width / (levelNum - 1) * i) + Vector3.right * (width / (levelNum - 1)) / 2, transform.rotation);
            text.transform.parent = this.transform;

            text.transform.position = transform.position - Vector3.right * (width / 2 - width / (levelNum - 1) * i) + Vector3.right * (width / (levelNum - 1)) / 2;
        }

        ps = new Text[levelNum];
        for (int i = 0; i < levelNum; i++) {
            GameObject text = Instantiate(scoreText, transform.position - Vector3.right * (width / 2 - width / (levelNum - 1) * i), transform.rotation);
            text.transform.parent = this.transform;
            
            text.GetComponent<Text>().color = Color.red;
            text.GetComponent<Text>().text = "0";
            ps[i] = text.GetComponent<Text>();
            text.GetComponent<Text>().color  = playerSpawner.instance.characterColors[i];


            text.transform.position = transform.position - Vector3.right * (width / 2 - width / (levelNum - 1) * i);
        }
            //ps[0].gameObject.SetActive(false);
    }

    void Update() {
        if (inputallowed && Input.anyKeyDown) {
            if (scoreCard.instance.highestScore() >= scoreCard.instance.gamesToWin) {
                Application.LoadLevel(0);
                Destroy(scoreCard.instance.gameObject);
            } else {
                //print(scoreCard.instance.highestScore());
                Application.LoadLevel(levelNum);
            }
        }
    }

    public void checkPlayersLeft() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 1) {
            players[0].GetComponent<Rigidbody2D>().gravityScale = 6;
            StopCoroutine("ending");
            StartCoroutine(ending(players[0].GetComponent<playertest>().playerNum));
            players[0].GetComponent<playertest>().enabled = false;
        } else if(players.Length == 0) {
            StartCoroutine(ending(-1));
        }
        
              
    }
    IEnumerator ending(int playerId) {
        
        yield return new WaitForSeconds(0.5f);
        cameraAnim.Play("endingTransition");
        yield return new WaitForSeconds(1);
        ps[0].gameObject.SetActive(true);

        if (playerId != -1) {
            

            for (int i = 0; i < scoreCard.instance.numOfPlayers; i++) {
                ps[i].text = "" + scoreCard.instance.playerScores[i];

            }

            yield return new WaitForSeconds(0.5f);
            scoreCard.instance.playerScores[playerId]++;
            audioManager.instance.Play(ding, 0.5f, 1);
        }
        inputallowed = true;
        for (int i = 0; i < scoreCard.instance.numOfPlayers;i++) {
            ps[i].text = "" + scoreCard.instance.playerScores[i];
        }

        if (scoreCard.instance.highestScore() >= scoreCard.instance.gamesToWin) {
            winParticles.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        anyKey.SetActive(true);


    }

    void OnDrawGizmos() {
        width = spacing * 4 * levelNum;
            
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 1, 1));

        for (int i = 0; i < levelNum; i++) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position - Vector3.right * (width / 2 - width / (levelNum - 1) * i), 25);
        }

        for (int i = 0; i < levelNum - 1; i++) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position - Vector3.right * (width / 2 - width / (levelNum - 1) * i) + Vector3.right * (width/(levelNum - 1))/2, new Vector3(10f,7f,1));
        }
    }
    
}
