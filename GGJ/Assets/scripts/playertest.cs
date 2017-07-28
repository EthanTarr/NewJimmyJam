using System.Collections;
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
    [HideInInspector] public Vector2 bounceDirection;

    public float slidingFloor;

    protected void Start() {

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        //Debug bounciness and charge
        maxSmashPower = scoreCard.instance.debugMaxSmash;
        bounceForce = scoreCard.instance.debugBounciness;

        anim.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

        baseColor = GetComponent<SpriteRenderer>().color;

        chargeVisualEffects(0);   
    }
    
    void LateUpdate() {

        float HorizInput = Input.GetAxis("Horizontal" + playerControl);
        anim.SetFloat("velocity", Mathf.Abs(HorizInput));

        checkGround();

        bounceDirection = new Vector2(Mathf.Lerp(bounceDirection.x, 0, Time.deltaTime * (smashing ? 25 : 2.5f)), Mathf.Lerp(bounceDirection.y, 0, Time.deltaTime * (smashing ? 25 : 45)));

        if (!smashing && !vunrabilityFrames && playerControl != "" && rigid.bodyType == RigidbodyType2D.Dynamic) {
            movement(HorizInput);
        }

        if (!smashing && !vunrabilityFrames) {
            rigid.velocity = new Vector2(xSpeed, rigid.velocity.y);
        }

        rigid.velocity += gravityDirection * 0.45f + bounceDirection;    
    }

    public bool jumped;
    protected void movement(float horizInput) {
        //Horizontal Movement
        bool touchingGround = checkGround();
        if (Mathf.Abs(horizInput) > 0.1f) {
            xSpeed += speed * horizInput / (accelerate * 4);
            anim.GetComponent<SpriteRenderer>().flipX = horizInput < 0;
            slopeCheck();
        } else {
            xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
        }
        xSpeed = Mathf.Clamp(xSpeed, -speed, speed);

        //Vertical Movement
        if (touchingGround) {
            transform.Translate(slidingFloor, 0, 0); //for levels with moving ground. Probably a neater way of doing this
            jumped = false;
            if (Input.GetButtonDown("Jump" + playerControl)) {
                rigid.velocity += new Vector2(rigid.velocity.x, maxJumpHeight);
                audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.93f, 1.05f));
                Invoke("jumpDelay", 0.05f);
            } 
        } else {
            if (canSmash) {
                if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f) {
                    StartCoroutine(dashOutOfCharge(Input.GetAxis("Dash" + playerControl), true));
                }

                if (Input.GetButtonDown("Smash" + playerControl) && !smashing) {
                    StartCoroutine(chargeSmash(Input.GetAxis("Horizontal" + playerControl)));
                }
            }
        }
        //shortHop
        if (Input.GetButtonUp("Jump" + playerControl) && rigid.velocity.y > minJumpHeight && jumped) {
            rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
        }
    }

    void jumpDelay() {
        jumped = true;
    }

    protected void chargeVisualEffects(float toggle) {
        GetComponent<SpriteRenderer>().color = Color.Lerp(baseColor, fullColor, toggle);
        chargeParticle.startSize = Mathf.Lerp(0, 0.4f, toggle);
        float changeSize = Mathf.Lerp(0, 0.75f, toggle);
        chargeParticle.transform.localScale = new Vector3(changeSize, changeSize, changeSize);
    }

    protected IEnumerator chargeSmash(float currentDirection) {
        bounceDirection.x = bounceDirection.x / 12;
        //bounceDirection.y = bounceDirection.y / 2;
        //rigid.velocity =  Vector2.right * rigid.velocity.x + bounceDirection;
        bounceDirection.y += Mathf.Max(0, rigid.velocity.y);
        print(bounceDirection.y);
        smashing = true;
        bool direction = GetComponent<SpriteRenderer>().flipX;
        float chargeValue = 0;
        SmashSpeed = minSmashSpeed;
        smashPower = minSmashPower;

        float initialY = Mathf.Max(0, rigid.velocity.y);

        while (chargeValue <= maxChargeTime) {
            
            float lerp = (chargeValue / maxChargeTime);
            chargeVisualEffects(lerp);

            SmashSpeed = Mathf.Lerp(minSmashSpeed, maxSmashSpeed, lerp);
            smashPower = Mathf.Lerp(minSmashPower, maxSmashPower, lerp);
            SmashCooldownTime = Mathf.Lerp(0.25f, maxSmashCooldownTime, lerp);

            if (!Input.GetButton("Smash" + playerControl)) {
                break;             
            }

            //Dash out of charge
            if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && Input.GetAxis("Dash" + playerControl) != currentDirection) {
                StartCoroutine(dashOutOfCharge(chargeValue, direction));
                yield break;
            }

            currentDirection = Input.GetAxis("Dash" + playerControl);
            chargeValue += Time.deltaTime;
            rigid.velocity = (Vector2.right * rigid.velocity.x) * 0.95f + Vector2.up * initialY + bounceDirection;

            initialY /= 1.07f;

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(smashAfterCharge(chargeValue));
    }

    protected IEnumerator dashOutOfCharge(float chargeValue, bool direction) {
        InvokeRepeating("SpawnTrail", 0, 0.035f);
        GetComponent<SpriteRenderer>().flipX = direction;
        bounceDirection += new Vector2(dashSpeed * Input.GetAxisRaw("Dash" + playerControl), 0);
        rigid.velocity = new Vector2(rigid.velocity.x, 1);

        smashing = false;
        chargeVisualEffects(0);
        SmashSpeed = 0;
        smashPower = 0;

        audioManager.instance.Play(this.cancel, 0.25f * (chargeValue / maxChargeTime));
        GameObject dashExpurosion = Instantiate(dashCancelParticle, transform.position, transform.rotation);
        dashExpurosion.GetComponent<ParticleSystem>().startColor = fullColor;
        Destroy(dashExpurosion, 1.5f);

        createDashParticle(2);

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
        rigid.velocity = new Vector2(0, -SmashSpeed + bounceDirection.y * 2);
        anim.SetBool("smashing", true);

        createDashParticle(1.5f);
        
        chargeVisualEffects(0);
    }

    public void createDashParticle(float lifetime) {
        GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
        dashTrail.transform.parent = this.transform;
        dashTrail.GetComponent<ParticleSystem>().startColor = fullColor;
        Destroy(dashTrail, lifetime);
    }

    [Space()]
    //this is really lazy to position raycasts to size
    public float downLazy = 0.33f;
	public bool checkGround() {
        bool grounded = false;
        for (int i = 0; i < 5; i++) {
            float dir = -1 + (i % 2) * 2;
            Vector3 downward = transform.position - transform.up * downLazy + transform.right * 0.1f * Mathf.Ceil(i/2f) * dir;
            RaycastHit2D hit = Physics2D.Raycast(downward, -transform.up, 0.4f);

            Debug.DrawRay(downward, -transform.up, Color.red);

            if (hit && hit.transform.GetComponent<SquareBehavior>() != null) {
                SquareBehavior square = hit.transform.GetComponent<SquareBehavior>();
                grounded = true;
                if (square.GetComponent<SquareBehavior>().TotalAmplitude - previousAmplitude > 0.1f) {
                    bounceDirection += Vector2.up * square.GetComponent<SquareBehavior>().TotalAmplitude;
                    //bounceDirection.y *= bounceForce / (square.transform.position.y < square.GetComponent<SquareBehavior>().initialY ? 15 : 4);
                    bounceDirection.y *= bounceForce;
                }
                previousAmplitude = square.GetComponent<SquareBehavior>().TotalAmplitude; // have the swuare itself keep track of its own acceleration or something
            } 
        }

        anim.SetBool("airborne", !grounded);
        if (!canMakeWave) {
            return grounded;
        }

        StartCoroutine("checkIfWaved");
        return grounded;
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

            float aboveMultiplyer = (other.transform.position.y + 0.5f < this.transform.position.y) ? 1: -1;

            GameObject colParticle = Instantiate(collisionParticle, other.contacts[0].point, transform.rotation);
            colParticle.GetComponent<ParticleSystem>().startColor = fullColor;
            audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 1, UnityEngine.Random.Range(0.96f, 1.03f));

            Vector2 dir = Vector2.zero;
            dir.x = other.relativeVelocity.x * (smashing ? 0.05f : 1);

            if (!checkGround())
                dir.x *= 1.25f;

            dir.x = Mathf.Min(Mathf.Abs(dir.x), 50) * Mathf.Sign(dir.x);

            dir.y = Mathf.Clamp(other.relativeVelocity.y,  aboveMultiplyer, smashing ? 25 : 15);

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

    protected void slopeCheck() {
        float littleHeight = 0.07f;
        float height = -1;

        for (int i = 1; i < 7; i++) {
            Debug.DrawRay(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x) * 0.3f, Color.red);
            RaycastHit2D slopeDetect = Physics2D.Raycast(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x), 0.3f, groundCheck);
            if (slopeDetect.collider != null) {
                height = i;
            }
        } if (height > -1 && height < 6) {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + littleHeight * height + 0.2f), Time.deltaTime * 12);
            checkGround(); //THIS MAY OR MAY NOT BE PART O THE PROBLEM. ONCE I STANDARDIZE WAVES WE MAY BE ABLE TO TAKE THIS OUT
        }
    }

    IEnumerator recovery(float recoveryTime) {
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

