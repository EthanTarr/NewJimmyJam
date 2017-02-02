using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {

    public string playerController;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 4;
    public float timeToJumpApex = 0.4f;

    float accelerationTimeAirborn = 0.3f;
    float acclerationTimeGrounded = 0.2f;
    public float moveSpeed = 6;

    float gravity = -20;
    float maxJumpVelocity = 8;
    float minJumpVelocity = 8;
    Vector3 velocity;
    float velocityXSmoothing;
    private Animator anim;

    Controller2D controller;

    void Start() {
        controller = GetComponent<Controller2D>();

        anim = GetComponent<Animator>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update() {

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        if (Input.GetButtonDown("Jump" + playerController) && controller.collisions.below) {
            velocity.y = maxJumpVelocity;
        }

        if (Input.GetButtonUp("Jump" + playerController) && velocity.y > minJumpVelocity) {
            velocity.y = minJumpVelocity;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal" + playerController), 0);

        float targetVelocityX = input.x * moveSpeed;
        if (Mathf.Sign(targetVelocityX) == Mathf.Sign(velocity.x)) {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? acclerationTimeGrounded : accelerationTimeAirborn);
        } else {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.5f : accelerationTimeAirborn);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        handleAnim(targetVelocityX);
    }

    void handleAnim(float xInput) {
        if (Mathf.Abs(velocity.x) > 0) {
            GetComponent<SpriteRenderer>().flipX = velocity.x < 0;
        }

        anim.SetFloat("velocity", Mathf.Abs(xInput));
        anim.SetBool("airborne", !controller.collisions.below);
    }
}
