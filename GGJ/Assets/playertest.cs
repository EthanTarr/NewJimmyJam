using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertest : MonoBehaviour {

    Rigidbody2D rigid;
    Animator anim;
    public LayerMask groundCheck;
    public Controls control;
    public int playerNum;
    [Space()]
    public float speed = 5;
    public float maxJumpHeight = 10;
    public float minJumpHeight;
    [Space()]
    public GameObject shockWave;
    public ParticleSystem dustParticles;

    public float minSmashSpped;
    public float maxSmashSpeed;

    public ParticleSystem chargeParticle;

    public float minPow = 5;
    public float power = 3;
    public float maxPow = 7;

    bool smashing;
    float smashReset;

    float timeTillRefresh = 2;

	private float previousAmplitude = 0;
	private int jumps = 0;
    public float bounceForce;

    bool laggin = false;
    bool canSmash = true;

    [Space()]
    public AudioClip[] softLanding;
    public AudioClip loadPower;
    public AudioClip smash;
    public AudioClip jump;
    public AudioClip charge;

    float xSpeed;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        baseColor = GetComponent<SpriteRenderer>().color;
        toggleCharge(0);        
    }

    Color baseColor;
    public Color fullColor;
    void toggleCharge(float toggle) {
        GetComponent<SpriteRenderer>().color = Color.Lerp(baseColor, fullColor, toggle);
        chargeParticle.startSize = Mathf.Lerp(0, 0.4f, toggle);
        float changeSize = Mathf.Lerp(0, 0.75f, toggle);
        chargeParticle.transform.localScale = new Vector3(changeSize, changeSize, changeSize);
    }

    void Update() {
        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();
		if (touchingGround) {
			jumps = 1;
		}
        if (!smashing && !laggin) {
            if (Mathf.Abs(Input.GetAxis("Horizontal" + playerNum)) > 0.1f) {
                xSpeed += speed * Input.GetAxis("Horizontal" + playerNum) / 8;
                GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal" + playerNum) < 0;
                anim.SetFloat("velocity", 1);
                slopeCheck();
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * 3);
            }

            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);
            rigid.velocity = new Vector2(xSpeed , rigid.velocity.y);


            if (Input.GetButtonDown("Jump" + playerNum) && touchingGround) {
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
                audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.97f, 1.03f));
            }

            if (Input.GetButtonUp("Jump" + playerNum) && rigid.velocity.y > minJumpHeight) {
                rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
            }


            if (Input.GetButtonDown("Smash" + playerNum) && canSmash && !smashing && !touchingGround) {
                StartCoroutine("chargeSmash");
            }
        }

        if (smashReset > 0) {
            smashReset -= 1;
        } else {
            smashing = false;
        }

		if (touchingGround) {
			//checkForWave ();
		}

    }

    float chargeLimit = 0.75f;
    IEnumerator chargeSmash() {
        speed /= 3;
        rigid.velocity = new Vector2(0, 0);
        rigid.gravityScale = 0;
        bool isChargin = true;
        float chargeValue = 0;
        
        while (isChargin && chargeValue < chargeLimit) {
            toggleCharge(1/(chargeValue*2 /chargeLimit));
            if (!Input.GetButton("Smash" + playerNum)) {         
                isChargin = false;
            }

            chargeValue += Time.deltaTime;
            yield return new WaitForEndOfFrame();
           

        }
        audioManager.instance.Play(charge, 0.5f * (chargeValue / chargeLimit));
        yield return new WaitForSeconds(0.05f);
        speed *= 3;
        chargeParticle.gameObject.transform.localScale = Vector3.zero;
        rigid.gravityScale = 3;
		minSmashSpped = chargeValue * 26.6f + 10;
		power = chargeValue * 4 + 4;
        rigid.velocity = new Vector2(0, -minSmashSpped);
        anim.SetBool("smashing", true);
        smashing = true;
        smashReset = 150;
        toggleCharge(0);
    }

	void checkForWave() {
		foreach (GameObject square in GameObject.FindGameObjectsWithTag("Floor")) {
			if (Mathf.Abs (square.transform.position.x - transform.position.x) < GameObject.Find("Managers").GetComponent<GameManager>().Square.transform.localScale.x) {
				if (square.GetComponent<SquareBehavior> ().TotalAmplitude - previousAmplitude > .1) {
					rigid.AddForce (new Vector2 (0, square.GetComponent<SquareBehavior> ().TotalAmplitude * bounceForce));
				}
				previousAmplitude = square.GetComponent<SquareBehavior> ().TotalAmplitude;
			}
		}
	}

    public bool checkGround() {
        RaycastHit2D hit = Physics2D.Raycast(GetComponent<Collider2D>().bounds.min, -transform.up, 0.5f, groundCheck);
        Debug.DrawLine(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.min - transform.up * 0.5f);
        anim.SetBool("airborne", hit == false);
        return hit;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor") && other.relativeVelocity.magnitude > 8)
        {
			float strength = Mathf.Clamp (other.relativeVelocity.magnitude / 40f, 0, .8f);
            if (smashing)
            {
                canSmash = false;
                Shake.instance.shake(2, 3);
                rigid.velocity = Vector3.zero;
                strength *= power;
                Color color = GetComponent<SpriteRenderer>().color;
                color.a = 0.75f;
                WaveGenerator.instance.makeWave(transform.position.x, strength, color, 7);
                audioManager.instance.Play(smash, 0.75f, UnityEngine.Random.Range(0.95f, 1.05f));
                StartCoroutine(recovery());
            }
            else
            {
                audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.05f, UnityEngine.Random.Range(0.96f, 1.03f));
                WaveGenerator.instance.makeWave(transform.position.x, strength, Color.white, 3);
            }
        } else if (other.gameObject.tag.Equals("Player")) {
            if (smashing)
            {
                smashing = false;
                Color color = GetComponent<SpriteRenderer>().color;
                color = GetComponent<SpriteRenderer>().color;
                color.a = 255f;
                GetComponent<SpriteRenderer>().color = color;
                anim.SetBool("smashing", false);
                canSmash = true;
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);

                StartCoroutine(other.gameObject.GetComponent<playertest>().knockedOut());
            } else {
                audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.05f, UnityEngine.Random.Range(0.96f, 1.03f));
            }
        }
    }

    IEnumerator knockedOut() {
        anim.SetBool("knockedOut", true);
        rigid.velocity = Vector2.zero;
        laggin = true;
        rigid.velocity = new Vector2(3, 10);
        yield return new WaitForSeconds(1);
        anim.SetBool("knockedOut", false);
        laggin = false;
    }

    void OnCollisionExit2D(Collision2D other) {
		checkForWave ();
    }

    void slopeCheck() {
        float littleHeight = 0.05f;
        float height = -1;

        for (int i = 0; i < 3; i++)
        {
            Debug.DrawRay(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x) * 0.6f, Color.red);
            RaycastHit2D slopeDetect = Physics2D.Raycast(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x), 0.6f, groundCheck);
            if (slopeDetect.collider != null) {
                height = i;
            }
        }
        if (height != 2 && height > -1) {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + littleHeight * height + 0.2f), Time.deltaTime * 4);
        }
    }

    IEnumerator recovery() {
        smashing = false;
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0.5f;
        GetComponent<SpriteRenderer>().color = color;
        laggin = true;
        yield return new WaitForSeconds(1);
        laggin = false;
        anim.SetBool("smashing", false);
        yield return new WaitForSeconds(timeTillRefresh);
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

[Serializable]
public class Controls
{
    public int playeriD;

    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode down;
}
