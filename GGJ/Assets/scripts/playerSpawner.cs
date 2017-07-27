using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawner : MonoBehaviour {

    public static playerSpawner instance;
    public float width;
    float numOfPlayers = 2;
    public GameObject playerPrefab;

    public bool showSpawners;

    public Color[] characterColors;

    void Awake() {
        instance = this;
        numOfPlayers = scoreCard.instance.numOfPlayers;

        for (int i = 0; i < numOfPlayers; i++) {
            Vector3 pos = transform.position - Vector3.right * (width / 2 - width / (numOfPlayers - 1) * i);
            GameObject player = Instantiate(scoreCard.instance.selectedCharacters[i], pos, transform.rotation);

            player.GetComponent<playertest>().playerNum = i;
            player.GetComponent<playertest>().fullColor = characterColors[i];
            player.GetComponent<SpriteRenderer>().color = characterColors[i];

            if (i < controllerHandler.controlOrder.Count)  {
                player.GetComponent<playertest>().playerControl = controllerHandler.controlOrder[i];
            } else {
                switch (i) {
                    case 0: player.GetComponent<playertest>().playerControl = "WASD";
                    break;

                    case 1:
                        player.GetComponent<playertest>().playerControl = "Arrow";
                        break;

                    case 2:
                        player.GetComponent<playertest>().playerControl = "Joy1";
                        break;

                    case 3:
                        player.GetComponent<playertest>().playerControl = "Joy2";
                        break;
                }
            }
        }
    }

    void OnDrawGizmos() {

        if (showSpawners)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, 1, 1));

            for (int i = 0; i < numOfPlayers; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position - Vector3.right * (width / 2 - width / (numOfPlayers - 1) * i), 0.5f);
            }
        }
    }
}
