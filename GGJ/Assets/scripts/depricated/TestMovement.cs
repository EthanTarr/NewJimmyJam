using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour {

    public float radius = 2f;
    public int speed = 2;
    public Transform centerOfGravity;
    private float i = 0;
    private float j = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //if (j % speed == 0)
        // {
        transform.RotateAround(centerOfGravity.position, new Vector3(0, 0, 1), speed * Time.deltaTime);
            //transform.Translate(new Vector3(Mathf.Cos(i * radius), Mathf.Sin(i * radius), 0));
            //i++;
        //}
        //j++;
    }
}
