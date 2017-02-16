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
    [HideInInspector] public Vector3 velocity;
    float velocityXSmoothing;
    private Animator anim;

    Controller2D controller;
    private float previousAmplitude = 0;
    public float bounceForce;

    bool fart;

    void Start() {
        controller = GetComponent<Controller2D>();

        anim = GetComponent<Animator>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update() {

        if (controller.collisions.above || controller.collisions.below) {
            if(!fart)
            velocity.y = 0;
        }

        if (Input.GetButtonDown("Jump" + playerController) && controller.collisions.below) {
            velocity.y += maxJumpVelocity;
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

        //checkForWave();

        handleAnim(targetVelocityX);
    }

    public void checkForWave() {
        fart = true;
        foreach (GameObject square in controller.collisions.belowCollisions) {
            //print(square.GetComponent<squareBehaviorv2>().TotalAmplitude - previousAmplitude);
            //if (Mathf.Abs(square.transform.position.x - transform.position.x) < GameObject.Find("Managers").GetComponent<GameManager>().Square.transform.localScale.x) {
                if (square.GetComponent<squareBehaviorv2>().TotalAmplitude - previousAmplitude < 0.01f) {
                    velocity = new Vector2(0, square.GetComponent<squareBehaviorv2>().TotalAmplitude * bounceForce);
                }

                previousAmplitude = square.GetComponent<squareBehaviorv2>().TotalAmplitude;
            
            //}
        }
        //fart = false;
        /* foreach (GameObject square in GameObject.FindGameObjectsWithTag("Floor")) {
             if (Mathf.Abs(square.transform.position.x - transform.position.x) < GameObject.Find("Managers").GetComponent<GameManager>().Square.transform.localScale.x) {
                 print("hio");
                 if (square.GetComponent<squareBehaviorv2>().TotalAmplitude - previousAmplitude < .1) {
                     velocity = new Vector2(velocity.x, square.GetComponent<squareBehaviorv2>().TotalAmplitude * bounceForce);
                 }
                 previousAmplitude = square.GetComponent<squareBehaviorv2>().TotalAmplitude;
             }
         } */
    }

    void handleAnim(float xInput) {
        if (Mathf.Abs(velocity.x) > 0) {
            GetComponent<SpriteRenderer>().flipX = velocity.x < 0;
        }

        anim.SetFloat("velocity", Mathf.Abs(xInput));
        anim.SetBool("airborne", !controller.collisions.below);
    }
}
