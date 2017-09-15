﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class stageSelectManager : MonoBehaviour {

    myInputModule input;
    playerController curPlayer;
    public GameObject stageSelect;
    public GameObject hitA;

    public string selectedLevel;

    // Use this for initialization
    void Start () {
        input = EventSystem.current.gameObject.GetComponent<myInputModule>();
        input.enabled = false;
	}

    IEnumerator turnOnInput(string controller)
    {
        yield return new WaitForSeconds(0.02f);
        input.enabled = true;
        input.horizontalAxis = "Horizontal" + controller;
        input.submitButton = "Enter" + controller;
        yield return new WaitForSeconds(0.25f);
        stageSelect.SetActive(true);
    }

    private void Update() {
        if (input.enabled  && curPlayer != null && Input.GetButtonDown(input.submitButton) && GameManager.instance.numOfPlayers >= 2) {
            StartCoroutine(screenTransition.instance.fadeOut(selectedLevel));
        }
        if (curPlayer != null && Input.GetButtonDown("Smash" + curPlayer.playerControl)) {
            curPlayer.active = true;
            curPlayer.transform.gameObject.layer = LayerMask.NameToLayer("Player");
            curPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            curPlayer = null;
            stageSelect.GetComponent<Animator>().Play("exitAnim");
            Invoke("turnOffInput",0.25f);
        }
    }

    public void changeSelectedLevel(string level) {
        selectedLevel = level;
    }

    void turnOffInput() {
        input.enabled = false;
        stageSelect.SetActive(false);
        hitA.SetActive(true);
        hitA.GetComponent<Animator>().Play("SelectAnim");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<playerController>() && curPlayer == null && !hitA.active) {
            hitA.SetActive(true);
            hitA.GetComponent<Animator>().Play("SelectAnim");
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<playerController>() && curPlayer == null) {
            if (Input.GetButtonDown("Enter" + collision.gameObject.GetComponent<playerController>().playerControl))
            {
                curPlayer = collision.gameObject.GetComponent<playerController>();
                curPlayer.transform.gameObject.layer = LayerMask.NameToLayer("background Objects");
                curPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                curPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                curPlayer.active = false;
                StartCoroutine( turnOnInput(curPlayer.playerControl));
                stageSelect.GetComponent<Animator>().Play("introAnim");
                hitA.SetActive(false);
            } else {
                hitA.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.GetComponent<playerController>()){
            hitA.SetActive(false);
        }
    }
}
