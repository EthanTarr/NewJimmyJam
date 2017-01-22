using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playertest : MonoBehaviour {

    Rigidbody2D rigid;
    Animator anim;
    public LayerMask groundCheck;
    public Controls control;
    [Space()]
    public float speed = 5;
    public float maxJumpHeight = 10;
    public float minJumpHeight;
    [Space()]
    public GameObject shockWave;
    public ParticleSystem dustParticles;

    public float smashSpeed;
    bool smashing;
	private float previousAmplitude = 0;
	private int jumps = 0;
    public float bounceForce;
	public GameObject Spike;
	private ArrayList spikePositions;
	private int spikes = 0;

    public bool laggin = false;
    public bool canSmash = true;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
		spikePositions = new ArrayList ();
		populateSpikePositions ();
    }
    float xSpeed;
    void Update() {
        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();
		if (touchingGround) {
			jumps = 1;
		}
        if (!smashing && !laggin) {
            if (Input.GetKey(control.right)) {
                xSpeed += speed / 8;
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetFloat("velocity", 1);
                slopeCheck();
            } else if (Input.GetKey(control.left)) {
                xSpeed -= speed / 8;
                GetComponent<SpriteRenderer>().flipX = true;
                anim.SetFloat("velocity", 1);
                slopeCheck();
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * 3);
            }

            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);
            rigid.velocity = new Vector2(xSpeed , rigid.velocity.y);


            if (Input.GetKeyDown(control.jump) && touchingGround) {
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
            }

            if (Input.GetKeyUp(control.jump) && rigid.velocity.y > minJumpHeight) {
                rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
            }


            if (Input.GetKeyDown(control.down) && canSmash && !smashing) {
                rigid.velocity = new Vector2(0, -smashSpeed);
                anim.SetBool("smashing", true);
                smashing = true;
            }
        }

		if (touchingGround) {
			checkForWave ();
		}

    }
		
	void populateSpikePositions() {
		foreach(GameObject spike in GameObject.FindGameObjectsWithTag("Spike")) {
			spikePositions.Add (spike);
			spikes++;
		}
	}

	void checkForWave() {
		foreach (GameObject square in GameObject.FindGameObjectsWithTag("Floor")) {
			if (Mathf.Abs (square.transform.position.x - transform.position.x) < .5f) {
				if (square.GetComponent<SquareBehavior> ().TotalAmplitude - previousAmplitude > .5) {
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

    float velocity;
    void LateUpdate() {
        velocity = rigid.velocity.y;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor") && other.relativeVelocity.magnitude > 8) {
            float strength = other.relativeVelocity.magnitude / 20f;
            if (smashing) {
                canSmash = false;
                GameObject shockwave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
                shockwave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, shockwave.GetComponent<SpriteRenderer>().color.a);
                Shake.instance.shake(2, 3);
                rigid.velocity = Vector3.zero;
                strength *= 5;
                WaveGenerator.instance.makeWave(transform.position.x, strength, GetComponent<SpriteRenderer>().color, 7);
                dustParticles.Emit(UnityEngine.Random.Range(5, 8));

				if (UnityEngine.Random.value < .03f) {
					int Fallingspike = UnityEngine.Random.Range (0, spikes);
					GameObject temp = (GameObject) Instantiate (Spike, ((GameObject) spikePositions [Fallingspike]).transform.position, Quaternion.identity);
					temp.GetComponent<Rigidbody2D> ().AddTorque (UnityEngine.Random.value * 30);
					Destroy((GameObject) spikePositions [Fallingspike]);
				}
                StartCoroutine(recovery());
            }
            else
            {
                WaveGenerator.instance.makeWave(transform.position.x, strength, Color.white, 3);
            }
        } else if (other.gameObject.tag.Equals("Player")) {
            if (smashing) {
                smashing = false;
                Color color = GetComponent<SpriteRenderer>().color;
                GameObject shockwave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
                shockwave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, shockwave.GetComponent<SpriteRenderer>().color.a);
                color = GetComponent<SpriteRenderer>().color;
                color.a = 255f;
                GetComponent<SpriteRenderer>().color = color;
                anim.SetBool("smashing", false);
                canSmash = true;
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);

                StartCoroutine(other.gameObject.GetComponent<playertest>().knockedOut());
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

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Floor")) {
            rigid.AddForce(new Vector2(0, other.gameObject.GetComponent<SquareBehavior>().velocity * (canSmash ? 5000 : 1000)));
        }
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
        yield return new WaitForSeconds(3);
        GameObject shockwave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
        shockwave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, shockwave.GetComponent<SpriteRenderer>().color.a);
        color = GetComponent<SpriteRenderer>().color;
        color.a = 255f;
        GetComponent<SpriteRenderer>().color = color;
        canSmash = true;
    }
}

[Serializable]
public class Controls {
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode down;
}
