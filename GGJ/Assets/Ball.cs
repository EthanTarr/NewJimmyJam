using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    protected Rigidbody2D rigid;
    public bool active;
    public LayerMask groundCheck;
    public Transform centerOfGravity;
    public Vector2 gravityDirection;
    public float gravityStrength;
    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        active = true;
    }

    void LateUpdate() {
        checkGround();

        bounceDirection = new Vector2(Mathf.Lerp(bounceDirection.x, 0, Time.deltaTime * 20), Mathf.Lerp(bounceDirection.y, 0, Time.deltaTime *  35));
        rigid.velocity += gravityDirection * gravityStrength;

        

        rigid.velocity += (Vector2)transform.up * bounceDirection.y;
        if (active)
        {
            rigid.velocity += (Vector2)transform.right * (bounceDirection.x);
        }
    }

    public float downLazy = 0.33f;
    private Vector2 bounceDirection;
    public float bounceForce;
    public bool checkGround() {
        bool grounded = false;
        for (int i = 0; i < 5; i++)
        {
            float dir = -1 + (i % 2) * 2;
            Vector3 downward = transform.position - transform.up * downLazy + transform.right * 0.1f * Mathf.Ceil(i / 2f) * dir;
            RaycastHit2D hit = Physics2D.Raycast(downward, -transform.up, 0.4f, groundCheck);

            Debug.DrawRay(downward, -transform.up, Color.red);

            if (hit) {
                grounded = true;
                if (hit.transform.GetComponent<SquareBehavior>() != null) {
                    SquareBehavior square = hit.transform.GetComponent<SquareBehavior>();
                    if (square.GetComponent<SquareBehavior>().TotalAmplitude > 2f) {
                        print("hoi");
                        bounceDirection += Vector2.up * square.GetComponent<SquareBehavior>().TotalAmplitude;
                        bounceDirection.y *= bounceForce;
                        square.GetComponent<SpriteRenderer>().color = Color.red;
                        grounded = false;
                    }
                    break;
                }
            }
        }
        return grounded;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor")) {
            //collisionWithFloor(other);
        } else if (other.gameObject.tag.Equals("Player"))
        {
            collisionWithPlayer(other);
        }

        else if (other.gameObject.tag.Equals("top")) {
            bounceDirection.y = 0;
        } else { //bounce off side walls
            bounceDirection.x = -1.5f;
        }
    }

    void collisionWithPlayer(Collision2D other){

        Vector2 dir = Vector2.zero;

        bool onTop = false;
        if (centerOfGravity == null)  {

            onTop = other.transform.position.y + downLazy / 2 < this.transform.position.y - downLazy;
        }
        else
        {
            onTop = Vector3.Distance(centerOfGravity.position, transform.position + downLazy / 2 * transform.up) < Vector3.Distance(other.transform.GetComponent<playerController>().centerOfGravity.position, other.transform.position - transform.GetComponent<playerController>().downLazy / 2 * other.transform.up);
        }
        float aboveMultiplyer = (onTop) ?  20  : 2;

        dir.y = Mathf.Clamp(transform.InverseTransformDirection(other.relativeVelocity).y, aboveMultiplyer, 50);
        

        dir.x = transform.InverseTransformDirection(other.relativeVelocity).x / 2;

        dir.y *= Mathf.Abs(dir.x) / 2;

        dir.x = Mathf.Min(Mathf.Abs(dir.x), 10) * Mathf.Sign(dir.x);
        bounceDirection += dir;
    }
}
