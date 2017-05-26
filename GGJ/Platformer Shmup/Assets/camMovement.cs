using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camMovement : MonoBehaviour {

    public GameObject focus;
    public float offset;
    public float speed;

	// Update is called once per frame
	void Update () {
        float targetPosition = Mathf.Lerp(transform.position.x, focus.transform.position.x +  offset, speed * Time.deltaTime);
        transform.position = new Vector3(targetPosition,transform.position.y, transform.position.z);
    }
}
