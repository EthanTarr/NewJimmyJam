﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cone : MonoBehaviour {

	void Start () {
        this.gameObject.SetActive(scoreCard.instance.ConeHeadMode) ;
	}

    void Update() {
        GetComponent<SpriteRenderer>().color = transform.parent.GetComponent<SpriteRenderer>().color;
    }

}