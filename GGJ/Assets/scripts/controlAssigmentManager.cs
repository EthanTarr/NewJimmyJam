using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlAssigmentManager : MonoBehaviour {

    public List<string> controllers = new List<string> { "Arrow", "WASD", "Joy1", "Joy2", "Joy3", "Joy4",  "Switch1" , "Switch2" , "Switch3" , "Switch4" };
    string[] inputs = new string[] { "Jump", "Smash" };
    public GameObject player;
    int setControls = 0;
    public Color[] colors;
    public List<playerController> players;

    [Header("Camera Position")]
    public Vector3 regularPosition;
    public Vector3 modifierMenuPos;
    bool modifierMenu;
    [Space()]
    public AudioClip elevatorDing;

    void Start() {
        //controllerHandler.controlOrder.Clear();
        grabOldControllerSet();
    }


    void grabOldControllerSet() {
        foreach (string control in controllerHandler.controlOrder) {
            controllers.Remove(control);
            setControls++;
        }

        if (Application.isEditor) {
            foreach (playerController player in FindObjectsOfType<playerController>()) {
                controllers.Remove(player.playerControl);
            }
        }
    }

    void Update() {
        if (controllerHandler.controlOrder.Count < 4 && Time.timeScale > 0) {
            foreach (string control in controllers) {
                bool inputFound = Mathf.Abs(Input.GetAxisRaw("Horizontal" + control)) > 0.1f;
                foreach (string input in inputs) {
                    if (Input.GetButton(input + control)) {
                        inputFound = true;
                    }
                }

                if (inputFound) {                                  
                    controllerHandler.controlOrder.Add(control);
                    controllers.Remove(control);
                    GameManager.instance.selectedCharacters[setControls] = player;

                    StartCoroutine(spawnPlayer(control, setControls));
                    setControls++;
                    GameManager.instance.numOfPlayers = setControls;
                    GameManager.instance.playerScores = new int[setControls];
                    break;
                }
            }
        }
    }

    public IEnumerator spawnPlayer(string control, int setControls) {
        audioManager.instance.Play(elevatorDing, 0.5f);
        yield return new WaitForSeconds(0.25f);
        transform.parent.gameObject.GetComponent<Animator>().Play("elevetorAnim");
        yield return new WaitForSeconds(0.25f);
        playerController newPlayer = Instantiate(player, transform.position, Quaternion.identity).GetComponent<playerController>();
        newPlayer.inLobby = true;
        newPlayer.playerControl = control;
        newPlayer.playerNum = setControls;
        newPlayer.GetComponent<SpriteRenderer>().color = colors[setControls];
        players.Add(newPlayer);
        print("player " + setControls + " mapped to " + newPlayer.playerControl);
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

    public void characterDropout(playerController player) {
        controllers.Add(player.playerControl);
        setControls--;
        GameManager.instance.numOfPlayers--;
        players.Remove(player);
        controllerHandler.controlOrder.Remove(player.playerControl);
        GameManager.instance.playerScores = new int[setControls];
        for(int i = 0; i < players.Count; i++) {
            //players[i].GetComponent<SpriteRenderer>().color = colors[i];
            //players[i].spriteAnim.GetComponent<SpriteRenderer>().color = colors[i];
            players[i].setColors(colors[i]);
            players[i].fullColor = colors[i];
        }
    }

    public void clearCharacterSelection() {
        controllerHandler.controlOrder.Clear();
        setControls = 0;
        players.Clear();
        controllers = new List<string> { "Arrow", "WASD", "Joy1", "Joy2", "Joy3", "Joy4" };
        GameManager.instance.numOfPlayers = 0;
        foreach (playerController player in FindObjectsOfType<playerController>()) {
            Destroy(player.gameObject);
        }
    }

    public void changeTargetSpawn(GameObject player) {
        this.player = player;
    }
}
