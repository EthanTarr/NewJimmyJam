﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class windZoneManager : MonoBehaviour {

    public static windZoneManager instance;
    private windController[] windControllers;
    public bool visible;
    public objectTranslate[] clouds;

    [Range(0, 360)]public float windAngle;

    [HideInInspector] public Vector2 windDirection;

    public bool randomDirection;

    public GameObject left;
    public GameObject right;

    private void Start() {
        if (randomDirection) {
            windAngle = 180 * Random.Range(0, 2);
        }
        setup();

        left.SetActive(windAngle != 180);
        right.SetActive(windAngle == 180);

        foreach (objectTranslate cloud in clouds)
            cloud.direction = windDirection;
    }

    private void Update() {
        if (!Application.isPlaying) {
            setup();
        }    
    }

    void setup() {
        instance = this;
        windControllers = GetComponentsInChildren<windController>();
        windDirection = DegreeToVector2(windAngle);
        foreach (windController wind in windControllers) {
            wind.GetComponent<SpriteRenderer>().enabled = visible;
            wind.windDirection = windDirection;
        }
    }

    public static Vector2 RadianToVector2(float radian) {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree) {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - (Vector3)windDirection * 1.5f, transform.position + (Vector3)windDirection * 1.5f);
        Gizmos.DrawWireSphere(transform.position + (Vector3)windDirection * 1.5f, 0.5f);
    }
}
