using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SquashAndStretch : MonoBehaviour
{

    public Vector2 originalScale;
    public GameObject sprite;
    private Rigidbody2D rigid;
    [Space()]
    public float animatedStretch;
    public float elasticForce = 25;
    public float collisionSquish = 0.75f;

    float verticalSquish;
    float verticalStretch;

    float lastVelocity;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 newScale = Vector2.one;
        if (Application.isPlaying) {
            float yVel = Mathf.Abs(rigid.velocity.y) * 0.05f;
            verticalStretch = Mathf.Lerp(verticalStretch, yVel, Time.fixedDeltaTime * elasticForce);
        
            verticalSquish = Mathf.Lerp(verticalSquish, 0, Time.fixedDeltaTime * 10);
            verticalStretch = Mathf.Max(verticalStretch, 0.1f);
            lastVelocity = rigid.velocity.y;
        }

        newScale.x = Mathf.Max(originalScale.x + animatedStretch - verticalStretch + verticalSquish + 0.1f, 0.5f);
        newScale.x = Mathf.Min(newScale.x, 1.25f);
        newScale.y = originalScale.y - animatedStretch + verticalStretch - verticalSquish;
        newScale.y = Mathf.Min(newScale.y, 2);
        newScale.y = Mathf.Max(newScale.y, 0.5f);
        sprite.transform.localScale = newScale;
        //sprite.transform.position = new Vector2(transform.position.x, transform.position.y - Mathf.Lerp(0.75f,0, Mathf.Min(newScale.y / 1, 1)));
        sprite.transform.position = transform.position - transform.up * Mathf.Lerp(0.75f, 0, Mathf.Min(newScale.y / 1, 1));
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor") && other.relativeVelocity.magnitude > 12) {
            verticalSquish = collisionSquish;
        }
    }
}
