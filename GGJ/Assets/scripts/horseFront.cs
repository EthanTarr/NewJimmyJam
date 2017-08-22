using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horseFront : playerController{

    public horseBack back;
    public SpriteRenderer horseHead;

    private void Start() {
        fullColor = GetComponent<SpriteRenderer>().color;
        fullColor.a = 0.75f;
        base.Start(); 
    }

    protected override void movement(float horizInput) {
        //Horizontal Movement
        bool touchingGround = checkGround();

        //Vertical Movement
        if (touchingGround) {
            jumped = false;
            if (Input.GetButtonDown("Jump" + playerControl)) {
                Invoke("queueBackJump", 0.15f);
            }
        }

        base.movement(horizInput);
    }

    void queueBackJump() {
        back.jumpQueued = true;
    }
}
