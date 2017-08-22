﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class window : MonoBehaviour {

    public float offset = 2;
    public float zOffset = -2;
    bool positioned;

    protected void Update(){
        getWindow();
    }

    void getWindow() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);

        Vector3 targetPos = transform.position;
        targetPos.y = hit.collider.transform.position.y + offset;
        targetPos.z = zOffset;
        transform.position = targetPos;
    }

}
