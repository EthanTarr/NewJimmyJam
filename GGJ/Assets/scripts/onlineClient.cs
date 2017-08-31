using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class onlineClient : NetworkBehaviour {

    public playerController targetPlayer;
    [SyncVar] public float HorizInput;

     void Start() {

        targetPlayer = playerSpawner.instance.players[playerSpawner.instance.queuedPlayer].GetComponent<playerController>();
        targetPlayer.client = this;
        playerSpawner.instance.queuedPlayer++;
    }

    void LateUpdate() {
       if (isLocalPlayer) {
            if (isLocalPlayer) {
                HorizInput = Input.GetAxis("HorizontalArrow");
                targetPlayer.CmdinputAudit(HorizInput);
            }
            
       }
    }
}
