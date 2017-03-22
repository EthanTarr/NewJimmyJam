using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugScript : MonoBehaviour {

	private float debugSetting;
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
        }


        if (Input.GetKeyDown(KeyCode.Z))
        {
            Time.timeScale = (Time.timeScale == 0.25f) ? 1 : 0.5f;
        }
    }
}
