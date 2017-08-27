using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class onlineAnim : NetworkBehaviour
{

    private void Update() {
        if (!transform.parent.GetComponent<NetworkBehaviour>().isLocalPlayer) {
            float HorizInput = transform.parent.GetComponent<Rigidbody2D>().velocity.x;
            GetComponent<Animator>().SetFloat("velocity", Mathf.Abs(HorizInput));
            GetComponent<SpriteRenderer>().flipX = HorizInput < 0 ? true : false; 
            GetComponent<Animator>().SetBool("airborne", !GetComponentInParent<playerController>().checkGround());
            GetComponent<Animator>().SetBool("smashing", transform.parent.GetComponent<Rigidbody2D>().velocity.y < - 6);
        }
    }
}
