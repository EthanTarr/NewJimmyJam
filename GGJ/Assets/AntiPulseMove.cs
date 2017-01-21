using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiPulseMove : MonoBehaviour {

	public float speed = 5;
	public float Amplitude = 1;
    public Color color = Color.white;

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (Time.deltaTime * -speed, 0, 0));
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        print("hoi");
        if (other.gameObject.GetComponent<SquareBehavior>() != null)
        {
            
            other.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
