using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlAssigmentManager : MonoBehaviour {

    List<string> controllers = new List<string> { "Arrow", "WASD", "Joy1", "Joy2", "Joy3", "Joy4" };
    string[] inputs = new string[] { "Jump", "Smash" };
    public GameObject player;
    int setControls = 0;
    public Color[] colors;


    void Start() {
        controllerHandler.controlOrder.Clear();
    }


    void Update() {
        if (controllerHandler.controlOrder.Count < 4) {
            foreach (string control in controllers) {
                bool inputFound = Mathf.Abs(Input.GetAxisRaw("Horizontal" + control)) > 0.1f;
                foreach (string input in inputs) {
                    if (Input.GetButton(input + control)) {
                        inputFound = true;
                    }
                }

                if (inputFound) {
                    playertest newPlayer = Instantiate(player, new Vector2(0, 3), Quaternion.identity).GetComponent<playertest>();             
                    newPlayer.playerControl = control;
                    newPlayer.playerNum = setControls;
                    newPlayer.GetComponent<SpriteRenderer>().color = colors[setControls];
                    controllerHandler.controlOrder.Add(control);
                    controllers.Remove(control);
                    setControls++;
                    scoreCard.instance.numOfPlayers = setControls;
                    print("player " + setControls + " mapped to " + newPlayer.playerControl);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Application.LoadLevel((setControls <= 2) ? 2 : 3);
        }
    }
}
