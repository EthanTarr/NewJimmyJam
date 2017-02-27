using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertest : MonoBehaviour {

    Rigidbody2D rigid;
    Animator anim;
    public LayerMask groundCheck;

    public string playerControl;
    public int playerNum;

    [Header("Control")]
    public float speed = 5;
    public float accelerate = 0;
    public float decelerate = 0;
    float xSpeed;
    [Space()]
    public float maxJumpHeight = 10;
    public float minJumpHeight;

    [Header("Smash Properties")]
    public float minSmashSpeed = 4;
    float SmashSpeed;
    public float maxSmashSpeed = 7; 
    [Space()]
    public float minSmashPower = 3;
    float smashPower = 3;
    public float maxSmashPower = 3;

    public float bounceForce;

    bool smashing;
    float smashReset;
    public float smashCooldownTime = 2;
	private float previousAmplitude = 0;
    public float maxChargeTime = 0.75f;

    [Header("Dash Cancel Properties")]
    public float dashSpeed = 7;

    bool vunrabilityFrames = false;
    bool canSmash = true;

    [Header("Visual Effects")]
    public GameObject shockWave;
    public ParticleSystem chargeParticle;
    Color baseColor;
    public Color fullColor;
    public GameObject dashCancelParticle;
    public GameObject dashTrailParticle;

    [Header("Sound Effects")]
    public AudioClip[] softLanding;
    public AudioClip loadPower;
    public AudioClip smash;
    public AudioClip jump;
    public AudioClip charge;
    public AudioClip cancel;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        baseColor = GetComponent<SpriteRenderer>().color;
        toggleCharge(0);
        if (controllerHandler.controlOrder.Count > playerNum) {
            playerControl = controllerHandler.controlOrder[playerNum];
        }       
    }

    void toggleCharge(float toggle) {
        GetComponent<SpriteRenderer>().color = Color.Lerp(baseColor, fullColor, toggle);
        chargeParticle.startSize = Mathf.Lerp(0, 0.4f, toggle);
        float changeSize = Mathf.Lerp(0, 0.75f, toggle);
        chargeParticle.transform.localScale = new Vector3(changeSize, changeSize, changeSize);
    }
    
    void Update() {

        if (playerControl == "") {
            return;
        }

        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();


        if (!smashing && !vunrabilityFrames) {
            if (Mathf.Abs(Input.GetAxis("Horizontal" + playerControl)) > 0.1f) {
                xSpeed += speed * Input.GetAxis("Horizontal" + playerControl) / (accelerate * 4);
                if (!vunrabilityFrames) {
                    GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal" + playerControl) < 0;
                }

                anim.SetFloat("velocity", 1);

                slopeCheck();
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
            }

            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);
            rigid.velocity = new Vector2(xSpeed , rigid.velocity.y);


            if (Input.GetButtonDown("Jump" + playerControl) && touchingGround) {
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
                audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.97f, 1.03f));
            }

            if (Input.GetButtonUp("Jump" + playerControl) && rigid.velocity.y > minJumpHeight) {
                rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
            }


            if (Input.GetButtonDown("Smash" + playerControl) && !isChargin && canSmash && !smashing && !touchingGround) {
                StartCoroutine(chargeSmash(Input.GetAxis("Horizontal" + playerControl)));
            }
        }

        if (smashReset > 0) {
            smashReset -= 1;
        } else {
            smashing = false;
        }
    }

    bool isChargin;
    IEnumerator chargeSmash(float currentDirection) {
        float originalSpeed = speed;
        speed /= 3;
        rigid.velocity = new Vector2(0, 0);
        rigid.gravityScale = 0;
        isChargin = true;
        bool cancel = false;
        bool direction = GetComponent<SpriteRenderer>().flipX;
        float chargeValue = 0;

        SmashSpeed = minSmashSpeed;
        smashPower = minSmashPower;

        while (isChargin && chargeValue < maxChargeTime) {
            float lerp = (chargeValue / maxChargeTime);
            toggleCharge(lerp);

            speed = Mathf.Lerp(speed, 0, lerp / 24);
            SmashSpeed = Mathf.Lerp(minSmashSpeed, maxSmashSpeed, lerp);
            smashPower = Mathf.Lerp(minSmashPower, maxSmashPower, lerp);
            if (!Input.GetButton("Smash" + playerControl)) {
                speed = originalSpeed;
                isChargin = false;
            }

            //Dash out of charge
            if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && Input.GetAxis("Dash" + playerControl) != currentDirection) {
                isChargin = false;
                cancel = true;
            } 

            chargeValue += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        speed = originalSpeed;
        if (!cancel) {
            audioManager.instance.Play(charge, 0.5f * (chargeValue / maxChargeTime));
            yield return new WaitForSeconds(0.05f);
            isChargin = false;
            chargeParticle.gameObject.transform.localScale = Vector3.zero;
            rigid.gravityScale = 3;
            rigid.velocity = new Vector2(0, -SmashSpeed);
            anim.SetBool("smashing", true);
            smashing = true;
            smashReset = 50;

            GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
            dashTrail.transform.parent = this.transform;
            dashTrail.GetComponent<ParticleSystem>().startColor = fullColor;
            Destroy(dashTrail, 1.5f);
            toggleCharge(0);
        } else {
            vunrabilityFrames = true;    
            GetComponent<SpriteRenderer>().flipX = direction;
            rigid.velocity = new Vector2(dashSpeed * Input.GetAxis("Dash" + playerControl), 1);
                  
            toggleCharge(0);
            rigid.gravityScale = 3;
            SmashSpeed = 0;
            smashPower = 0;
            smashReset = 50;

            audioManager.instance.Play(this.cancel, 0.25f * (chargeValue / maxChargeTime));
            GameObject dashExpurosion = Instantiate(dashCancelParticle, transform.position, transform.rotation);
            dashExpurosion.GetComponent<ParticleSystem>().startColor = fullColor;
            Destroy(dashExpurosion, 1.5f);
            GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
            dashTrail.transform.parent = this.transform;
            dashTrail.GetComponent<ParticleSystem>().startColor = fullColor;
            Destroy(dashTrail, 1.5f);

            //canSmash = false;
            //StartCoroutine(recovery());
            yield return new WaitForSeconds(0.5f);
            
            chargeParticle.gameObject.transform.localScale = new Vector3(rigid.velocity.x,0);
            vunrabilityFrames = false;
        }
    }

	void checkForWave() {
        
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("Floor")) {
			if (Mathf.Abs (square.transform.position.x - transform.position.x) < GameObject.Find("Managers").GetComponent<GameManager>().Square.transform.localScale.x) {
				if (square.GetComponent<SquareBehavior> ().TotalAmplitude - previousAmplitude > .1) {
                    rigid.AddForce(new Vector2(0, square.GetComponent<SquareBehavior>().TotalAmplitude * bounceForce));
                }
				previousAmplitude = square.GetComponent<SquareBehavior> ().TotalAmplitude;
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
            if (other.relativeVelocity.magnitude > 8) {
                float strength = Mathf.Clamp(other.relativeVelocity.magnitude / 40f, 0, .8f);
                if (smashing) {
                    canSmash = false;
                    Shake.instance.shake(2, 3);
                    rigid.velocity = Vector3.zero;
                    strength *= smashPower;
                    Color color = GetComponent<SpriteRenderer>().color;
                    color.a = 0.75f;
                    WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength, color, 7);
                    audioManager.instance.Play(smash, 0.75f, UnityEngine.Random.Range(0.95f, 1.05f));

                    SmashSpeed = 0;
                    smashPower = 0;

                    StartCoroutine(recovery());
                } else {
                    rigid.velocity = Vector3.zero;
                    audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.05f, UnityEngine.Random.Range(0.96f, 1.03f));

                    if (canMakeWave)
                        WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength, Color.white, 3);
                }
            } 
        } else if (other.gameObject.tag.Equals("Player")) {
            if(other.gameObject.transform.position.y < transform.position.y)
                rigid.velocity = new Vector2(rigid.velocity.x, (smashing || isChargin) ? 15 : 5);
        }
    }

    IEnumerator knockedOut() {
        anim.SetBool("knockedOut", true);
        rigid.velocity = Vector2.zero;
        vunrabilityFrames = true;
        rigid.velocity = new Vector2(3, 10);
        yield return new WaitForSeconds(1);
        anim.SetBool("knockedOut", false);
        vunrabilityFrames = false;
    }

    bool canMakeWave = true;
    IEnumerator checkIfWaved() {
        canMakeWave = false;
        yield return new WaitForSeconds(0.75f);
        canMakeWave = true;
    }

    void OnCollisionExit2D(Collision2D other) {
        checkForWave ();
    }

    void slopeCheck() {
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

    IEnumerator recovery() {
        smashing = false;
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0.5f;
        GetComponent<SpriteRenderer>().color = color;
        vunrabilityFrames = true;
        yield return new WaitForSeconds(1);
        vunrabilityFrames = false;
        anim.SetBool("smashing", false);
        yield return new WaitForSeconds(smashCooldownTime);
        GameObject wave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
        wave.transform.SetParent(transform);
        wave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, wave.GetComponent<SpriteRenderer>().color.a);

        float time = 0;

        color = GetComponent<SpriteRenderer>().color;
        color.a = 255f;
        GetComponent<SpriteRenderer>().color = color;
        audioManager.instance.Play(loadPower, 0.5f, UnityEngine.Random.Range(0.96f, 1.03f));
        canSmash = true;
    }
}

