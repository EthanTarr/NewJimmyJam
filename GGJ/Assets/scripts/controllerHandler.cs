using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerHandler : MonoBehaviour
{

    public static List<string> controllers = new List<string> { "Arrow", "WASD", "Joy1", "Joy2", "Joy3", "Joy4" };
    public static string[] inputs = new string[] { "Jump", "Smash" };
    public playertest[] players;
    int setControls = 0;

    void Update()
    {
        if (setControls < players.Length) {
            foreach (string control in controllers) {
                bool inputFound = Mathf.Abs(Input.GetAxisRaw("Horizontal" + control)) > 0.1f;
                foreach (string input in inputs) {
                    if (Input.GetButton(input + control)) {
                        inputFound = true;
                    }
                }

                if (inputFound) {
                    players[setControls].playerControl = control;
                    controllers.Remove(control);
                    setControls++;
                    break;
                }
            }
        }
    }
}
