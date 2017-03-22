using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //scoreCard.instance.isConeHeadMode();
        scoreCard.instance.gamesToWin = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayGame() {
        print("hoihoihoi");
		Application.LoadLevel(1);
	}
}
