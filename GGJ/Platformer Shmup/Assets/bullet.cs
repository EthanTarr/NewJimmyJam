﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

    public float speed;

    void Update() {
        transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.up * speed, Time.deltaTime * 7);
    }
}
