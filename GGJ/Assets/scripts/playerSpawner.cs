using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class playerSpawner : NetworkBehaviour {

    public static playerSpawner instance;
    public float width;
    float numOfPlayers = 2;
    public GameObject playerPrefab;

    public bool showSpawners;
    public bool online;
    public bool lobby;

    public Color[] characterColors;

    //onlineManagement
    public playerController[] players;
    [SyncVar] public int queuedPlayer;

    public override void OnStartServer()
    {
        players = new playerController[(int)numOfPlayers];
        if (online)
        {
            for (int i = 0; i < numOfPlayers; i++)
            {
                Vector3 pos = transform.position - Vector3.right * (width / 2 - width / (numOfPlayers - 1) * i);
                GameObject player = Instantiate(playerPrefab, pos, transform.rotation);

                player.GetComponent<playerController>().playerNum = i;
                player.GetComponent<SpriteRenderer>().color = characterColors[i]; 
                players[i] = player.GetComponent<playerController>();
                NetworkServer.Spawn(player);
            }
        }
    }

    void Awake() { // this might throw things for a loop if its start instead of Awake. keep an eye out to see if stuff happens
        instance = this;

        numOfPlayers = GameManager.instance.numOfPlayers;
        players = new playerController[(int)numOfPlayers];
        for (int i = 0; i < numOfPlayers; i++) {
            Vector3 pos = transform.position - Vector3.right * (width / 2 - width / (numOfPlayers - 1) * i);
            GameObject player = Instantiate(GameManager.instance.selectedCharacters[i], pos, transform.rotation);

            player.GetComponent<playerController>().playerNum = i;
            //player.GetComponent<playertest>().fullColor = characterColors[i];
            player.GetComponent<SpriteRenderer>().color = characterColors[i];

            if (!lobby)
            {
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            players[i] = player.GetComponent<playerController>();
            if (i < controllerHandler.controlOrder.Count)  {
                player.GetComponent<playerController>().playerControl = controllerHandler.controlOrder[i];
            } else {
                switch (i) {
                    case 0: player.GetComponent<playerController>().playerControl = "WASD";
                    break;

                    case 1:
                        player.GetComponent<playerController>().playerControl = "Arrow";
                        break;

                    case 2:
                        player.GetComponent<playerController>().playerControl = "Joy1";
                        break;

                    case 3:
                        player.GetComponent<playerController>().playerControl = "Joy2";
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
