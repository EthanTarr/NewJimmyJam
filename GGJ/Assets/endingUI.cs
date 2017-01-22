using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endingUI : MonoBehaviour {

    public Animator cameraAnim;
    public static endingUI instance;

    void Start() {
        instance = this;
    }
}
