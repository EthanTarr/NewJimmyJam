using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class height : MonoBehaviour {

    private Slider slider;
    public float goalHeight;
    float currentHeight;
    public static height instance;

    public GameObject exit;

    void Start() {
        instance = this;
        slider = GetComponent<Slider>();

        Instantiate(exit, new Vector3(0, goalHeight), Quaternion.identity);
    }

    void Update() {

        slider.value = currentHeight / goalHeight;
    }
}
