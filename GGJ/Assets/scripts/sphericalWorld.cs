using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphericalWorld : MonoBehaviour {

    public Vector2 startingPosition;
    public Vector2 velocity;

    void Start() {
        startingPosition = transform.position;
    }

    void Update() {
        transform.position = Vector2.Lerp(transform.position, startingPosition, Time.deltaTime * 5);

        velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * 6);
        float gravity = 0;

        transform.position += (Vector3)velocity;

        if (Input.GetKeyDown(KeyCode.Space)) {
            applyForceToPlanet(transform.right, 1f);
        }
    }

    public void applyForceToPlanet(Vector2 direction, float strength) {
        velocity += direction * strength;
    }
}
