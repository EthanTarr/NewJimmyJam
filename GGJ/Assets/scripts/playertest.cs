﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertest : MonoBehaviour {

    protected Rigidbody2D rigid;
    public Animator anim;
    public LayerMask groundCheck;

    public string playerControl;
    public int playerNum;

    [Header("Control")]
    public float speed = 5;
    public float accelerate = 0;
    public float decelerate = 0;
    protected float xSpeed;
    [Space()]
    public float maxJumpHeight = 10;
    public float minJumpHeight;

    [Header("Gravity")]
    public float gravityStrength;
    public Vector2 gravityDirection;

    [Header("Smash Properties")]
    public float minSmashSpeed = 4;
    protected float SmashSpeed;
    public float maxSmashSpeed = 7; 
    [Space()]
    public float minSmashPower = 3;
    protected float smashPower = 3;
    public float maxSmashPower = 3;

    public float bounceForce;

    protected bool smashing;


    public float maxSmashCooldownTime = 2;
    protected float SmashCooldownTime = 0;

    protected float previousAmplitude = 0;
    public float maxChargeTime = 0.75f;

    bool canMakeWave = true;

    [Header("Dash Cancel Properties")]
    public float dashSpeed = 7;
    public float dashCharge = 0.35f;

    protected bool vunrabilityFrames = false;
    protected bool canSmash = true;

    [Header("Visual Effects")]
    public GameObject shockWave;
    public ParticleSystem chargeParticle;
    protected Color baseColor;
    public Color fullColor;
    public GameObject dashCancelParticle;
    public GameObject dashTrailParticle;
    public GameObject deathParticle;
    public GameObject collisionParticle;

    public AudioClip deathExplosion;

    [Header("Sound Effects")]
    public AudioClip[] softLanding;
    public AudioClip loadPower;
    public AudioClip smash;
    public AudioClip jump;
    public AudioClip charge;
    public AudioClip cancel;

    [Header("Bounciness")]
    public float bounciness;
    protected Vector2 bounceDirection;

    public float slidingFloor;

    protected void Start() {

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        //Debug bounciness and charge
        maxSmashPower = scoreCard.instance.debugMaxSmash;
        bounceForce = scoreCard.instance.debugBounciness;

        anim.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

        baseColor = GetComponent<SpriteRenderer>().color;

        toggleCharge(0);   
    }

    protected void toggleCharge(float toggle) {
        GetComponent<SpriteRenderer>().color = Color.Lerp(baseColor, fullColor, toggle);
        chargeParticle.startSize = Mathf.Lerp(0, 0.4f, toggle);
        float changeSize = Mathf.Lerp(0, 0.75f, toggle);
        chargeParticle.transform.localScale = new Vector3(changeSize, changeSize, changeSize);
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            Application.LoadLevel("Jose's other scene");
        }
        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();

        bounceDirection = new Vector2(Mathf.Lerp(bounceDirection.x, 0, Time.deltaTime * 2.5f), Mathf.Lerp(bounceDirection.y, 0, Time.deltaTime * (smashing ? 5 : 45)));

        if (!smashing && !vunrabilityFrames && playerControl != "" && rigid.bodyType == RigidbodyType2D.Dynamic) {
            if (Mathf.Abs(Input.GetAxis("Horizontal" + playerControl)) > 0.1f) {
                xSpeed += speed * Input.GetAxis("Horizontal" + playerControl) / (accelerate * 4);
                anim.GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal" + playerControl) < 0;
                anim.SetFloat("velocity", 1);
                slopeCheck();
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
            }

            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);

            if (touchingGround) {
                transform.Translate(slidingFloor,0,0);
            }

            if (Input.GetButtonDown("Jump" + playerControl) && touchingGround) {
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
                audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.93f, 1.05f));
            }

            if (Input.GetButtonUp("Jump" + playerControl) && rigid.velocity.y > minJumpHeight) {
                rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
            }

            if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && canSmash && !touchingGround) {
               StartCoroutine(dashOutOfCharge(Input.GetAxis("Dash" + playerControl), true));
            }

            if (Input.GetButtonDown("Smash" + playerControl)
                    && canSmash && !smashing && !touchingGround) {
                StartCoroutine(chargeSmash(Input.GetAxis("Horizontal" + playerControl)));
            }        
        }

        rigid.velocity += gravityDirection * 0.45f;
          
        if (!smashing && !vunrabilityFrames) {
            rigid.velocity = new Vector2(xSpeed, rigid.velocity.y) + bounceDirection;
        }
    }

    protected IEnumerator chargeSmash(float currentDirection) {
        bounceDirection = bounceDirection / 8;
        rigid.velocity =  Vector2.right * rigid.velocity.x + bounceDirection;

        
        smashing = true;
        bool direction = GetComponent<SpriteRenderer>().flipX;
        float chargeValue = 0;
        SmashSpeed = minSmashSpeed;
        smashPower = minSmashPower;

        while (chargeValue <= maxChargeTime) {
            
            float lerp = (chargeValue / maxChargeTime);
            toggleCharge(lerp);

            SmashSpeed = Mathf.Lerp(minSmashSpeed, maxSmashSpeed, lerp);
            smashPower = Mathf.Lerp(minSmashPower, maxSmashPower, lerp);
            SmashCooldownTime = Mathf.Lerp(0.25f, maxSmashCooldownTime, lerp);

            if (!Input.GetButton("Smash" + playerControl)) {
                StartCoroutine(smashAfterCharge(chargeValue));
                yield break;            
            }

            //Dash out of charge
            if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && Input.GetAxis("Dash" + playerControl) != currentDirection) {
                StartCoroutine(dashOutOfCharge(chargeValue, direction));
                yield break;
            }

            currentDirection = Input.GetAxis("Dash" + playerControl);
            chargeValue += Time.deltaTime;
            rigid.velocity = (Vector2.right * rigid.velocity.x) * 0.95f + bounceDirection;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(smashAfterCharge(chargeValue));
    }

    protected IEnumerator dashOutOfCharge(float chargeValue, bool direction) {
        InvokeRepeating("SpawnTrail", 0, 0.035f);
        //vunrabilityFrames = true;
        GetComponent<SpriteRenderer>().flipX = direction;
        bounceDirection += new Vector2(dashSpeed * Input.GetAxisRaw("Dash" + playerControl), 0);
        rigid.velocity = new Vector2(rigid.velocity.x, 1);
        print(bounceDirection);

        smashing = false;
        toggleCharge(0);

        SmashSpeed = 0;
        smashPower = 0;

        audioManager.instance.Play(this.cancel, 0.25f * (chargeValue / maxChargeTime));
        GameObject dashExpurosion = Instantiate(dashCancelParticle, transform.position, transform.rotation);
        dashExpurosion.GetComponent<ParticleSystem>().startColor = fullColor;
        Destroy(dashExpurosion, 1.5f);

        GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
        dashTrail.transform.parent = this.transform;
        dashTrail.GetComponent<ParticleSystem>().startColor = fullColor;
        Destroy(dashTrail, 2f);

        canSmash = false;
        StartCoroutine(recovery(dashCharge));
        
        vunrabilityFrames = false;
        yield return new WaitForSeconds(0.25f);

        chargeParticle.gameObject.transform.localScale = new Vector3(rigid.velocity.x, 0);
        CancelInvoke("SpawnTrail");
    }

    protected IEnumerator smashAfterCharge(float chargeValue) {
        if (chargeValue > maxChargeTime / 2)
            InvokeRepeating("SpawnTrail", 0, 0.035f);

        audioManager.instance.Play(charge, 0.5f * (chargeValue / maxChargeTime));
        yield return new WaitForSeconds(0.05f);
        chargeParticle.gameObject.transform.localScale = Vector3.zero;
        rigid.velocity = new Vector2(0, -SmashSpeed + bounceDirection.y * 10);
        anim.SetBool("smashing", true);

        GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
        dashTrail.transform.parent = this.transform;
        dashTrail.GetComponent<ParticleSystem>().startColor = fullColor;
        Destroy(dashTrail, 1.5f);
        toggleCharge(0);
    }

    public float downLazy = 0.33f;
	void checkForWave() {

        for (int i = 0; i < 4; i++) {
            Vector3 downward = transform.position - transform.up * downLazy - transform.right * 0.25f + transform.right * 0.15f * i;
            RaycastHit2D hit = Physics2D.Raycast(downward, -transform.up, 0.1f);

            Debug.DrawRay(downward, -transform.up, Color.red);

            if (hit && hit.transform.GetComponent<SquareBehavior>() != null) {
                SquareBehavior square = hit.transform.GetComponent<SquareBehavior>();
                if (square.GetComponent<SquareBehavior>().TotalAmplitude - previousAmplitude > .1) {
                    if (square.transform.position.y < square.GetComponent<SquareBehavior>().initialY + 0.5f) {
                        bounceDirection += Vector2.up * square.GetComponent<SquareBehavior>().TotalAmplitude * bounceForce / 12;
                    } else {
                        bounceDirection += Vector2.up * square.GetComponent<SquareBehavior>().TotalAmplitude * bounceForce / 6;
                    }
                }
                previousAmplitude = square.GetComponent<SquareBehavior>().TotalAmplitude;
            } 
        }

        if (!canMakeWave) {
            return;
        }

        StartCoroutine("checkIfWaved");
    }

    public bool checkGround() {
        RaycastHit2D hit = Physics2D.Raycast(GetComponent<Collider2D>().bounds.min, -transform.up, 0.5f, groundCheck);
        Debug.DrawLine(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.min - transform.up * 0.5f);
        anim.SetBool("airborne", hit == false);
        return hit;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor")) {
            float strength = Mathf.Clamp(other.relativeVelocity.magnitude / 40f, 0, .8f);
            if (smashing) {     
                StopCoroutine("smashAfterCharge");
                CancelInvoke("SpawnTrail");
                Shake.instance.shake(2, 3);
                rigid.velocity = Vector3.zero;
                strength *= smashPower;
                Color color = GetComponent<SpriteRenderer>().color;
                color.a = 0.75f;
                WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength, color, 7);
                audioManager.instance.Play(smash, 0.75f, UnityEngine.Random.Range(0.95f, 1.05f));

                SmashSpeed = 0;
                smashPower = 0;
                StartCoroutine(recovery(SmashCooldownTime));               
            } else if (other.relativeVelocity.magnitude > 8) {
                audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.25f, UnityEngine.Random.Range(0.96f, 1.03f));
                if (canMakeWave)
                    WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength, Color.white, 3);
            }
            
        } else if (other.gameObject.tag.Equals("Player")) {

            float aboveMultiplyer = (other.transform.position.y + 0.5f < this.transform.position.y) ? 10 : -10;

            GameObject colParticle = Instantiate(collisionParticle, other.contacts[0].point, transform.rotation);
            colParticle.GetComponent<ParticleSystem>().startColor = fullColor;
            audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 1, UnityEngine.Random.Range(0.96f, 1.03f));

            Vector2 dir;
            dir.x = other.relativeVelocity.x * (smashing ? 0.05f : 1);
            dir.x = Mathf.Min(Mathf.Abs(dir.x), 50) * Mathf.Sign(dir.x);

            dir.y = Mathf.Clamp(other.relativeVelocity.y * (smashing ? 3 : 1f), aboveMultiplyer, smashing ? 25 : 15);
            bounceDirection += dir * bounciness;
        } else if (other.gameObject.tag.Equals("Spike") && !alreadyDead) {
            die();
        }
    }

    IEnumerator checkIfWaved() {
        canMakeWave = false;
        yield return new WaitForSeconds(1.5f);
        canMakeWave = true;
    }

    void OnCollisionExit2D(Collision2D other) {
        checkForWave ();
    }

    protected void slopeCheck() {
        float littleHeight = 0.05f;
        float height = -1;

        for (int i = 1; i < 3; i++) {
            Debug.DrawRay(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x) * 0.6f, Color.red);
            RaycastHit2D slopeDetect = Physics2D.Raycast(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x), 0.6f, groundCheck);
            if (slopeDetect.collider != null) {
                height = i;
            }
        } if (height != 2 && height > -1) {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + littleHeight * height + 0.2f), Time.deltaTime * 4);
        }
    }

    IEnumerator recovery(float recoveryTime) {

        //if (smashing) {
            smashing = false;
            canSmash = false;
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 0.5f;
            anim.GetComponent<SpriteRenderer>().color = color;
            vunrabilityFrames = true;

            yield return new WaitForSeconds(recoveryTime);

            vunrabilityFrames = false;
            anim.SetBool("smashing", false);
            yield return new WaitForSeconds(recoveryTime);
            GameObject wave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
            wave.transform.SetParent(transform);
            wave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, wave.GetComponent<SpriteRenderer>().color.a);
            color = GetComponent<SpriteRenderer>().color;
            color.a = 255f;
            anim.GetComponent<SpriteRenderer>().color = color;
            audioManager.instance.Play(loadPower, 0.5f, UnityEngine.Random.Range(0.96f, 1.03f));
            SmashCooldownTime = 0.25f;
            canSmash = true;
        //}
    }

    protected bool alreadyDead;
    public virtual void die() {
        alreadyDead = true;
        audioManager.instance.Play(deathExplosion, 0.5f, UnityEngine.Random.Range(0.96f, 1.04f));
        GameObject particle = Instantiate(deathParticle, transform.position, Quaternion.identity) as GameObject;
        particle.GetComponent<ParticleSystem>().startColor = fullColor;
        GetComponent<SpriteRenderer>().color = fullColor;
        Shake.instance.shake(2, 3);
        Destroy(this.gameObject);
        endingUI.instance.Invoke("checkPlayersLeft", 0.25f);     
    }

    void SpawnTrail() {
        GameObject trailPart = new GameObject();
        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
        trailPartRenderer.sprite = anim.GetComponent<SpriteRenderer>().sprite;
        trailPartRenderer.flipX = anim.GetComponent<SpriteRenderer>().flipX;

        Color targetColor = fullColor;
        targetColor.a = 0.5f;
        trailPartRenderer.color = targetColor;

        trailPart.transform.position = transform.position;
        trailPart.transform.localScale = transform.localScale;
        Destroy(trailPart, 2f); // replace 0.5f with needed lifeTime

        StartCoroutine("FadeTrailPart", trailPartRenderer);
    }

    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer) {
        while (trailPartRenderer != null && trailPartRenderer.color.a > 0) {
            Color color = trailPartRenderer.color;
            color.a -= 0.01f; // replace 0.5f with needed alpha decrement
            trailPartRenderer.color = color;

            yield return new WaitForEndOfFrame();
        }
    }
}

