using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlAssigmentManager : MonoBehaviour {

    List<string> controllers = new List<string> { "Arrow", "WASD", "Joy1", "Joy2", "Joy3", "Joy4" };
    string[] inputs = new string[] { "Jump", "Smash" };
    public GameObject player;
    int setControls = 0;
    public Color[] colors;

    public string selectedLevel;

    [Header("Camera Position")]
    public Vector3 regularPosition;
    public Vector3 modifierMenuPos;
    bool modifierMenu;

    void Start() {
        controllerHandler.controlOrder.Clear();
    }


    void Update() {

        //if (Input.GetKeyDown(KeyCode.Q)) {
        //    swapToModifierMenu();
        //}

        if (controllerHandler.controlOrder.Count < 4) {
            foreach (string control in controllers) {
                bool inputFound = Mathf.Abs(Input.GetAxisRaw("Horizontal" + control)) > 0.1f;
                foreach (string input in inputs) {
                    if (Input.GetButton(input + control)) {
                        inputFound = true;
                    }
                }

                if (inputFound) {
                    playerController newPlayer = Instantiate(player, transform.position, Quaternion.identity).GetComponent<playerController>();             
                    newPlayer.playerControl = control;
                    newPlayer.playerNum = setControls;
                    newPlayer.GetComponent<SpriteRenderer>().color = colors[setControls];
                    controllerHandler.controlOrder.Add(control);
                    controllers.Remove(control);
                    GameManager.instance.selectedCharacters[setControls] = player;
                    
                    setControls++;
                    GameManager.instance.numOfPlayers = setControls;
                    GameManager.instance.playerScores = new int[setControls];

                    print("player " + setControls + " mapped to " + newPlayer.playerControl);
                    break;
                }
            }
        }

        if (Input.GetButtonDown("Enter") && GameManager.instance.numOfPlayers >= 2) {
            print("hoi");
            Application.LoadLevel(selectedLevel);
        }
    }

    public void swapToModifierMenu() {
        clearCharacterSelection();

        StopAllCoroutines();
        StartCoroutine(lerpCamera(!modifierMenu ? modifierMenuPos : regularPosition));
        modifierMenu = !modifierMenu;
    }

    IEnumerator lerpCamera(Vector3 targetPos) {
        while (Vector3.Distance(Camera.main.transform.position, targetPos) > 0.1f) {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * 7);
            yield return new WaitForEndOfFrame();
        }

        Camera.main.transform.position = targetPos;
    }

    public void clearCharacterSelection() {
        controllerHandler.controlOrder.Clear();
        setControls = 0;
        controllers = new List<string> { "Arrow", "WASD", "Joy1", "Joy2", "Joy3", "Joy4" };
        GameManager.instance.numOfPlayers = 0;
        foreach (playerController player in FindObjectsOfType<playerController>()) {
            Destroy(player.gameObject);
        }
    }

    public void changeSelectedLevel(string level) {
        selectedLevel = level;
    }

    public void changeTargetSpawn(GameObject player) {
        this.player = player;
    }
}
