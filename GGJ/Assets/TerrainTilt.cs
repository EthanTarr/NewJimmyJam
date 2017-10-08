using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTilt : MonoBehaviour {

    public float rotation;
    float targetRotation;
    public float rotationMax;
    public float size;
    public float smashOffset;
    windController[] slipperiess;

    private void Start() {
        Invoke("getSlippery", 0.25f);
    }

    void getSlippery() {
        slipperiess = FindObjectsOfType<windController>();
    }

    private void Update() {
        playerController[] players = FindObjectsOfType<playerController>();
        targetRotation = 0;
        foreach (playerController player in players) {
            if (!player.checkGround()) {
                continue;
            }
            float direction = -Mathf.Sign(player.transform.position.x - transform.position.x);
            float magnitude = Mathf.Lerp(0,1, Mathf.Abs(player.transform.position.x - transform.position.x) / ((size + 0.2f)/2));

            targetRotation += direction * magnitude;

            targetRotation = Mathf.Clamp(targetRotation, -rotationMax, rotationMax);
        }

        smashOffset = Mathf.Lerp(smashOffset, 0, Time.deltaTime * 7);
        rotation = Mathf.Lerp(rotation, targetRotation + smashOffset, Time.deltaTime * 3);
        //rotation = Mathf.Clamp(rotation, -rotationMax, rotationMax);

        foreach (windController i in slipperiess) {
            i.speed = Mathf.Sign(rotation) * Mathf.Lerp(0, 0.05f, Mathf.Abs(rotation) / rotationMax);
        }

        transform.eulerAngles = Vector3.forward * rotation;
    }

    public void applySmashForce(Vector2 position, float strength) {
        float direction = -Mathf.Sign(position.x - transform.position.x);
        float magnitude = Mathf.Lerp(0.01f, 1, Mathf.Abs(position.x - transform.position.x) / ((size + 0.2f) / 2));
        strength = Mathf.Lerp(1,1.5f, strength/0.8f);
        smashOffset = direction * 7 * magnitude * strength;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(size, 1));
    }
}
