using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerHandler : MonoBehaviour {

    public Control test;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[System.Serializable]
public class Control{
    public int playerId;

    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode down;
}