using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour, IComparable<playerController> {

    protected Rigidbody2D rigid;
    public SpriteScript spriteAnim;
    public LayerMask groundCheck;

    public string playerControl;
    public int playerNum;
    bool canDoubleJump;
    protected bool touchingGround;
    public float topWidth;

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
    public Transform centerOfGravity;
    bool fastFall;
    bool canFastFall;

    [Header("Smash Properties")]
    public float minSmashSpeed = 4;
    protected float SmashSpeed;
    public float maxSmashSpeed = 7; 
    [Space()]
    public float minSmashPower = 3;
    protected float smashPower = 3;
    public float maxSmashPower = 3;

    public float bounceForce;
    [HideInInspector] public bool jumped;

    protected float maxSmashVulnerabilityTime;
    [HideInInspector] public bool smashing;
    protected float SmashCooldownTime = 0;
    public float waveSpeed = 7;
    public float maxChargeTime = 0.75f;
    float chargeValue;

    protected bool vunrabilityFrames = false;

    protected float previousAmplitude = 0;
    bool canMakeWave = true;
    [SyncVar]  protected bool canSmash = true;

    [Header("Dash Properties")]
    public float dashSpeed = 7;
    public float dashCharge = 0.35f;
    protected bool canDash = true;
    protected float dashDirection;
    protected float dashDecel;

    [Header("Visual Effects")]

    public GameObject shockWave;
    public ParticleSystem chargeParticle;
    protected Color baseColor;
    public Color fullColor;
    public GameObject dashCancelParticle;
    public GameObject dashTrailParticle;
    public GameObject deathParticle;
    public GameObject collisionParticle;
    public GameObject chargeCircle;

    public AudioClip deathExplosion;

    [Header("Sound Effects")]
    public AudioClip[] softLanding;
    public AudioClip loadPower;
    public AudioClip smash;
    public AudioClip jump;
    public AudioClip charge;
    public AudioClip cancel;
    public AudioClip extraBounce;

    [Header("Bounciness")]
    public float bounciness;
    [SyncVar] [HideInInspector] public Vector2 bounceDirection;
     
    private SquashAndStretch sqetch;

    [Header("Modifiers")]
    public bool airControl;
    public bool seperateDashCooldown;
    public bool canDashOnGround;
    public bool instantBounceKill;
    public bool fullChargeInvinc;
    public bool holdMaxSmash;
    public bool tightDash;
    public bool doubleJump;

    [Space()]
    //online stuffs
    public bool onlinePlayer;
    public GameObject pulse;
    public GameObject antiPulse;
    public onlineClient client;

    [HideInInspector] public bool inLobby;

    protected virtual void Start() {
        rigid = GetComponent<Rigidbody2D>();
        spriteAnim = GetComponentInChildren<SpriteScript>();

        active = true;
         
        changeModifiers();


        setColors(GetComponent<SpriteRenderer>().color);
        
        sqetch = GetComponent<SquashAndStretch>();
        chargeParticle.startColor = Color.Lerp(baseColor, Color.white, 0.5f);

        chargeVisualEffects(0);
        chargeCircle.transform.localScale = Vector3.zero;
    }

    public void setColors(Color baseColor) {
        spriteAnim.GetComponent<SpriteRenderer>().color = baseColor;
        this.baseColor = baseColor;
        chargeParticle.startColor = Color.Lerp(baseColor, Color.white, 0.5f);
    }

    protected void changeModifiers() {
        airControl = GameManager.instance.airControl;
        seperateDashCooldown = GameManager.instance.seperateDashCooldown;
        canDashOnGround = GameManager.instance.canDashOnGround;
        maxSmashPower = GameManager.instance.maxSmashPower;
        bounceForce = GameManager.instance.bounciness;
        maxSmashSpeed = GameManager.instance.maxSmashSpeed;
        fullChargeInvinc = GameManager.instance.fullChargeInvinc;
        holdMaxSmash = GameManager.instance.holdMaxSmash;
        maxChargeTime = GameManager.instance.maxChargeTime;
        tightDash = GameManager.instance.tightDash;
        instantBounceKill = GameManager.instance.instantBounceKill;
        dashSpeed = GameManager.instance.dashDistance;
        doubleJump = GameManager.instance.doubleJump;
    }

    void LateUpdate() {
        if (onlinePlayer) {
            if (client != null) {
                CmdinputAudit(client.HorizInput);
            }
        } else {
            float HorizInput = Input.GetAxis("Horizontal" + playerControl);
            CmdinputAudit(HorizInput);
        }

        if (inLobby && Input.GetButtonDown("Dropout" + playerControl)) {
            StartCoroutine(dropoutOfLobby());
        }
    }

    // movement for online multiplayer
    public virtual void CmdinputAudit(float HorizInput) {
        checkGround();

        bounceDirection = new Vector2(Mathf.Lerp(bounceDirection.x, 0, Time.deltaTime * (smashing ? 25 : 2.5f)), Mathf.Lerp(bounceDirection.y, 0, Time.deltaTime * (smashing ? 2 : 35)));
        dashDirection = Mathf.Lerp(dashDirection, 0, Time.deltaTime * dashDecel);

        if (!smashing && !vunrabilityFrames && playerControl != "" && rigid.bodyType == RigidbodyType2D.Dynamic) {
            movement(HorizInput);
        } if (!smashing && !vunrabilityFrames && active) {
            rigid.velocity = transform.TransformDirection(new Vector2(xSpeed, transform.InverseTransformDirection(rigid.velocity).y));
        }

        if ((canSmash || !seperateDashCooldown || !tightDash))
        {
            if (centerOfGravity == null) {
                rigid.velocity += gravityDirection * gravityStrength;
                if (Input.GetAxis("Vertical" + playerControl) < -0.7f) {
                    rigid.velocity += gravityDirection * 0.25f;
                }
            } else {
                Vector2 gravityDirection = (-transform.position + centerOfGravity.position).normalized;

                //rigid.AddForce(gravityDirection * gravityStrength);
                rigid.velocity -= (Vector2)transform.up * gravityStrength;

                Vector2 dirUp = -gravityDirection;

                this.transform.up = Vector2.Lerp(this.transform.up, dirUp, Time.deltaTime * 500);
            }
        }

        rigid.velocity += (Vector2)transform.up * bounceDirection.y;
        if (active) {
            rigid.velocity += (Vector2)transform.right * (dashDirection + bounceDirection.x);
        }
    }

     public bool active = true;
    protected virtual void movement(float horizInput) {
        //Horizontal Movement
        touchingGround = checkGround();

        if (active) {
            if (Mathf.Abs(horizInput) > 0.1f && (canSmash || !seperateDashCooldown)) {
                xSpeed += speed * horizInput / (accelerate * 4);
                spriteAnim.GetComponent<SpriteRenderer>().flipX = horizInput < 0;
                slopeCheck();
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
            }
            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);

            //Vertical Movement
            if (touchingGround) {
                jumped = false;
                canDoubleJump = true;
                fastFall = false;
                if (Input.GetButtonDown("Jump" + playerControl) && (canSmash || !seperateDashCooldown || !tightDash)) {
                    //rigid.velocity += new Vector2(rigid.velocity.x, maxJumpHeight);
                    rigid.velocity += (Vector2)transform.up * maxJumpHeight;
                    audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.93f, 1.05f));
                    //checkGround();
                    Invoke("jumpDelay", 0.05f);
                }

                if (Input.GetAxis("Vertical" + playerControl) >= -0.7f)
                {
                    canFastFall = true;
                    if (sqetch.animatedStretch > 0.1f) {
                        StartCoroutine(squishControl(0f, 0.01f));
                    }
                } else {
                    if (sqetch.animatedStretch < 0.5f) {
                        audioManager.instance.Play(softLanding[0], 0.05f);
                        StartCoroutine(squishControl(0.5f, 0.05f));
                    }
                }

                //dashingOnGround
                if (canDash && Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && (canDashOnGround && canSmash)) {
                    StartCoroutine(dash(Input.GetAxis("Dash" + playerControl), true, true));
                }
            } else {
                if (canSmash) {
                    if (Input.GetButtonDown("Jump" + playerControl) && doubleJump && canDoubleJump) {
                        canDoubleJump = false;
                        rigid.velocity += (Vector2)transform.up * maxJumpHeight / 1.5f;
                        //rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight/1.5f + bounceDirection.y);
                        audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.93f, 1.05f));

                        GameObject dashExpurosion = Instantiate(dashCancelParticle, transform.position, transform.rotation);
                        dashExpurosion.GetComponent<ParticleSystem>().startColor = spriteAnim.GetComponent<SpriteRenderer>().color;
                        Destroy(dashExpurosion, 1.5f);

                        checkGround();
                    }


                    //fastFall
                    /*
                    if (Input.GetAxis("Vertical" + playerControl) < -0.7 && !fastFall && canFastFall && !smashing) {
                        fastFall = true;
                        canFastFall = false;
                    }
                    */

                    if (canDash && Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && !smashing)
                    {
                        StartCoroutine(dash(Input.GetAxis("Dash" + playerControl), true, false));
                    }

                    if (Input.GetButtonDown("Smash" + playerControl) && !smashing)
                    {
                        StartCoroutine(chargeSmash(Input.GetAxis("Horizontal" + playerControl)));
                    }
                    
                }
            }

            //shortHop
            if (Input.GetButtonUp("Jump" + playerControl) && transform.InverseTransformDirection(rigid.velocity).y > minJumpHeight && jumped) {
                rigid.velocity = (Vector2)transform.up * minJumpHeight;
            }
            spriteAnimationManager(horizInput, touchingGround);
        }
    }

    protected void spriteAnimationManager(float HorizInput, bool touchingGround) {
        if (!touchingGround) {
            spriteAnim.SetAnimation("jump");
            return;
        }

        if (vunrabilityFrames) {
            spriteAnim.SetAnimation("crush");
            return;
        }

        spriteAnim.SetAnimation(Mathf.Abs(HorizInput) > 0.1f ? "walk" : "idle");
    }

    void jumpDelay() {
        jumped = true;
    }

    IEnumerator dropoutOfLobby() {
        float t = 1.5f;
        active = false;
        while (t > 0) {
            t -= Time.deltaTime;
            if (!Input.GetButton("Dropout" + playerControl)) {
                yield break;
            }
            yield return null;
        }

        audioManager.instance.Play(deathExplosion, 0.5f, UnityEngine.Random.Range(0.96f, 1.04f));

        GameObject particle = Instantiate(deathParticle, transform.position, Quaternion.identity) as GameObject;
        particle.GetComponent<ParticleSystem>().startColor = Color.Lerp(baseColor, Color.white, 0.5f);

        GetComponent<SpriteRenderer>().color = fullColor;

        FindObjectOfType<controlAssigmentManager>().characterDropout(this);
        Destroy(this.gameObject);
    }

    protected void chargeVisualEffects(float toggle) {
        spriteAnim.GetComponent<SpriteRenderer>().color = Color.Lerp(baseColor, fullColor, toggle);
        chargeParticle.startSize = Mathf.Lerp(0.25f, 0.75f, toggle * 10);
        float changeSize = Mathf.Lerp(0, 1, toggle * 2);
        chargeCircle.transform.localScale = Vector3.one * Mathf.Lerp(0.6f,0.1f, toggle);

        Color chargeColor= GetComponent<SpriteRenderer>().color;
        chargeColor.a = Mathf.Lerp(0,0.5f,toggle);
        chargeCircle.GetComponent<SpriteRenderer>().color = chargeColor;

        chargeParticle.transform.localScale = new Vector3(changeSize, changeSize, changeSize);
    }

    protected IEnumerator chargeSmash(float currentDirection) {
        bounceDirection.x = bounceDirection.x / 12;    
        bounceDirection.y *= 10;
        
        dashDirection = 0;

        bool direction = GetComponent<SpriteRenderer>().flipX;

        fastFall = false;

        smashing = true;
        bool maxed = false;
        chargeValue = 0;

        SmashSpeed = minSmashSpeed;
        smashPower = minSmashPower;
        float hold = maxChargeTime + (holdMaxSmash ? 0.5f : 0);

        float initialY = Mathf.Max(0, transform.InverseTransformDirection(rigid.velocity).y);

        while (chargeValue <= hold) {
            float lerp = (chargeValue / maxChargeTime);
            chargeVisualEffects(lerp);

            SmashSpeed = Mathf.Lerp(minSmashSpeed, maxSmashSpeed, lerp);
            smashPower = Mathf.Lerp(minSmashPower, maxSmashPower, lerp);
            SmashCooldownTime = Mathf.Lerp(0.25f, maxSmashVulnerabilityTime, lerp);

            if (!Input.GetButton("Smash" + playerControl) || smashing == false) {
                audioManager.instance.Play(charge, 0.5f * (chargeValue / maxChargeTime));
                break;
            }

            //Dash out of charge
            if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && Input.GetAxis("Dash" + playerControl) != currentDirection && canDash) {
                chargeValue = 0;
                chargeCircle.transform.localScale = Vector3.zero;
                StartCoroutine(dash(chargeValue, direction, false));
                yield break;
            }

            currentDirection = Input.GetAxis("Dash" + playerControl);
            chargeValue += Time.deltaTime;

            if (chargeValue >= maxChargeTime && !maxed) {
                audioManager.instance.Play(charge, 0.5f * (chargeValue / maxChargeTime), 1.1f);

                GameObject wave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
                wave.transform.SetParent(transform);
                wave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, wave.GetComponent<SpriteRenderer>().color.a);

                Color color = GetComponent<SpriteRenderer>().color;
                color.a = 0.5f;
                spriteAnim.GetComponent<SpriteRenderer>().color = color;
                color = GetComponent<SpriteRenderer>().color;
                color.a = 255f;
                spriteAnim.GetComponent<SpriteRenderer>().color = color;

                maxed = true;
            }

            rigid.velocity = ((Vector2)transform.right * transform.InverseTransformDirection(rigid.velocity).x) * 0.95f + (Vector2)transform.up * initialY + (Vector2)transform.InverseTransformDirection((Vector3)bounceDirection);

            initialY /= 1.1f;
            if (airControl)
            {
                rigid.velocity += (Vector2)transform.right * Input.GetAxis("Horizontal" + playerControl) * 0.2f;
            }

            yield return new WaitForEndOfFrame();
        }

        chargeValue = Mathf.Min(chargeValue, maxChargeTime);
        StartCoroutine(smashOutOFCharge(chargeValue));
    }

    protected IEnumerator dash(float chargeValue, bool direction) {
        print("dashing");
        yield return dash(chargeValue, direction, false);
        chargeCircle.transform.localScale = Vector3.zero;
        chargeValue = 0;
    }

    protected IEnumerator dash(float chargeValue, bool direction, bool onGround) {
        InvokeRepeating("SpawnTrail", 0, 0.035f);
        GetComponent<SpriteRenderer>().flipX = direction;

        float dir = Input.GetAxis("Horizontal" + playerControl);
       // print(dir);
        if (Mathf.Abs(dir) < 0.5f)
           dir = (spriteAnim.GetComponent<SpriteRenderer>().flipX ? -1 : 1);

        if (Input.GetAxis("Vertical" + playerControl) <= -0.9f) {
            dir *= 1.5f;
        }

        dashDirection += dashSpeed * dir/ (onGround ? 1.5f : 1);
        dashDirection *= 1.5f;
        dashDecel = tightDash ? 7f : 5f;

        rigid.velocity = transform.TransformDirection(new Vector2(0, onGround ? -10 :  Mathf.Max(0, transform.InverseTransformDirection(rigid.velocity).y) / 2));

        smashing = false;
        chargeVisualEffects(0);
        SmashSpeed = 0;
        smashPower = 0;

        spriteAnim.GetComponent<SpriteRenderer>().color = changeOpacity(spriteAnim.GetComponent<SpriteRenderer>().color, 0.5f);

        audioManager.instance.Play(this.cancel, 0.25f);

        GameObject dashExpurosion = Instantiate(dashCancelParticle, transform.position, transform.rotation);
        dashExpurosion.GetComponent<ParticleSystem>().startColor = spriteAnim.GetComponent<SpriteRenderer>().color;
        Destroy(dashExpurosion, 1.5f);

        createDashParticle(2);

        if (!seperateDashCooldown) {
            canSmash = false;
            StartCoroutine(recovery(dashCharge));
            vunrabilityFrames = false;

            yield return new WaitForSeconds(0.25f);

            chargeParticle.gameObject.transform.localScale = new Vector3(rigid.velocity.x, 0);
            CancelInvoke("SpawnTrail");
        } else {
            canSmash = false;
            canDash = false;
            vunrabilityFrames = false;

            float j = 0;
            while (j < 0.1f) {
                j += Time.deltaTime;
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y/1.1f +  bounceDirection.y);
                yield return new WaitForEndOfFrame();
            }

            canSmash = true;
            chargeParticle.gameObject.transform.localScale = new Vector3(rigid.velocity.x, 0);
            CancelInvoke("SpawnTrail");

            yield return new WaitForSeconds(1f);
            canDash = true;
        }
        spriteAnim.GetComponent<SpriteRenderer>().color = changeOpacity(spriteAnim.GetComponent<SpriteRenderer>().color, 225);
        audioManager.instance.Play(loadPower, 0.5f, UnityEngine.Random.Range(0.96f, 1.03f));
    }

    protected IEnumerator smashOutOFCharge(float chargeValue) {
        if (chargeValue > maxChargeTime / 2)
            InvokeRepeating("SpawnTrail", 0, 0.035f);

        yield return new WaitForSeconds(0.05f);
        
        rigid.velocity = transform.TransformDirection(new Vector3(0, -SmashSpeed));
        bounceDirection.y = 0;
        spriteAnim.SetAnimation("smash");
        createDashParticle(1.5f);
        
        chargeVisualEffects(0);
        chargeCircle.transform.localScale = Vector3.zero;
    }

    public void createDashParticle(float lifetime) {
        GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
        dashTrail.transform.parent = this.transform;
        dashTrail.GetComponent<ParticleSystem>().startColor = changeOpacity(baseColor, 0.75f);
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
            RaycastHit2D hit = Physics2D.Raycast(downward, -transform.up, 0.4f, groundCheck);

            Debug.DrawRay(downward, -transform.up, Color.red);

            if (hit) {         
                grounded = true;
                if (hit.transform.GetComponent<SquareBehavior>() != null) {
                    SquareBehavior square = hit.transform.GetComponent<SquareBehavior>();


                    if (square.GetComponent<SquareBehavior>().TotalAmplitude > 2f) {
                        //print(square.GetComponent<SquareBehavior>().TotalAmplitude);
                        bounceDirection += Vector2.up * square.GetComponent<SquareBehavior>().TotalAmplitude;
                        bounceDirection.y *= bounceForce / (jumped ? 3 : 1);
                        square.GetComponent<SpriteRenderer>().color = Color.red;
                        grounded = false;
                    }
                    break;
                    //previousAmplitude = square.GetComponent<SquareBehavior>().TotalAmplitude; // have the swuare itself keep track of its own acceleration or something
                }
            }        
        }
        
        //if (!canMakeWave) {
        //    StartCoroutine("checkIfWaved");
        //}
        return grounded;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor")) {
            collisionWithFloor(other);
        } else if (other.gameObject.GetComponent<Ball>() != null) {
            collisionWithBall(other);
        } else if (other.gameObject.tag.Equals("Player")) {
            collisionWithPlayer(other);
        }
        else if (other.gameObject.tag.Equals("Spike") && !alreadyDead) {
            die();
        } else if (other.gameObject.tag.Equals("top")) {
            bounceDirection.y = 0;
        } else { //bounce off side walls
            GameObject colParticle = Instantiate(collisionParticle, transform.position, transform.rotation);
            colParticle.GetComponent<ParticleSystem>().startColor = baseColor;
            Destroy(colParticle, 0.25f);
            audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.75f, UnityEngine.Random.Range(0.96f, 1.03f));
            bounceDirection.x *= -0.75f;
            dashDirection *= -0.75f;
        }
    }

    void collisionWithBall(Collision2D other) {
        GameObject colParticle = Instantiate(collisionParticle, other.contacts[0].point, transform.rotation);
        colParticle.GetComponent<ParticleSystem>().startColor = baseColor;
        Destroy(colParticle, 0.75f);

        //if (smashing && other.transform.GetComponent<playerController>().touchingGround) {
        //    WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, 0.75f, Color.white, chargeValue >= maxChargeTime ? 5 : 3, null);
        //}

        audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 1, UnityEngine.Random.Range(0.96f, 1.03f));

        Vector2 dir = Vector2.zero;

        bool onTop = false;
        if (centerOfGravity == null) {
            onTop = other.transform.position.y + downLazy / 2 < this.transform.position.y - downLazy;
        } else {
            onTop = Vector3.Distance(centerOfGravity.position, transform.position + downLazy / 2 * transform.up) < Vector3.Distance(other.transform.GetComponent<Ball>().centerOfGravity.position, other.transform.position - transform.GetComponent<Ball>().downLazy / 2 * other.transform.up);
        }
        float aboveMultiplyer = (onTop) ? (instantBounceKill ? 20 : 10) : 0;

        dir.y = Mathf.Clamp(transform.InverseTransformDirection(other.relativeVelocity).y, aboveMultiplyer, 50);
        dir.y *= (smashing ? 1 : 1.5f) * (chargeValue > 0.1f ? 2 : 1);

        dir.x = transform.InverseTransformDirection(other.relativeVelocity).x * (smashing ? 0.2f : 1);
        if (!touchingGround)
        {
            dir.x *= 1.25f;
        }
        dir.x = Mathf.Min(Mathf.Abs(dir.x), 50) * Mathf.Sign(dir.x);

        if (onTop)
        {
            colParticle.GetComponent<ParticleSystem>().startSize = 1.5f;
            colParticle.GetComponent<ParticleSystem>().startSpeed = 70f;
            StartCoroutine(headBoopSquish());
            audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 1, UnityEngine.Random.Range(0.96f, 1.03f));
        }
        bounceDirection += dir;
    }

    void collisionWithFloor(Collision2D other) {
        float strength = Mathf.Clamp(other.relativeVelocity.magnitude / 40f, 0, .8f);
        if (smashing) {
            StopCoroutine("chargeSmash");
            StopCoroutine("smashAfterCharge");
            CancelInvoke("SpawnTrail");
            canSmash = false;

            if (Shake.instance != null)
                Shake.instance.shake(2, 3);

            
            rigid.velocity = Vector3.zero;
            // *= smashPower;
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 0.75f;

            if (onlinePlayer) {
                CmdMakeWave(transform.position + Vector3.up * -1, strength, color, waveSpeed);
            } else {
                //print(strength);
                //WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength * smashPower, color, Mathf.Lerp(5,10, strength / 0.8f), null);
                //print(chargeValue >= maxChargeTime);
                WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength * smashPower, color, chargeValue >= maxChargeTime ? 12 : 8, null);
            }

            audioManager.instance.Play(smash, 0.75f, UnityEngine.Random.Range(0.95f, 1.05f));
            SmashSpeed = 0;
            smashPower = 0;
            chargeValue = 0;
            StartCoroutine(recovery(SmashCooldownTime));

            if (FindObjectOfType<TerrainTilt>() != null) {
                FindObjectOfType<TerrainTilt>().applySmashForce(this.transform.position, strength);
            }
        } else {
            /*
            if (other.relativeVelocity.magnitude > 12 && !onlinePlayer) {
                audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.05f, UnityEngine.Random.Range(0.96f, 1.03f));
                if (canMakeWave) {
                    if (onlinePlayer)
                    {
                        CmdMakeWave(transform.position + Vector3.up * -1, strength / 2, Color.white, 1.5f);
                    }
                    else
                    {
                        WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength / 2, Color.white, 1.5f, null);
                    }
                } else {
                    StartCoroutine("checkIfWaved");
                }
            }
            */
        }


    }

    void collisionWithPlayer(Collision2D other) {
        GameObject colParticle = Instantiate(collisionParticle, other.contacts[0].point, transform.rotation);
        colParticle.GetComponent<ParticleSystem>().startColor = baseColor;
        Destroy(colParticle, 0.75f);

        if (smashing && other.transform.GetComponent<playerController>().touchingGround) {
            WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, 0.75f, Color.white, chargeValue >= maxChargeTime ? 5 : 3, null);
        }

        //if fully charged, and th mod is on, player cannot be bounced
        if (chargeValue >= maxChargeTime && fullChargeInvinc) {
            float pushBack = other.relativeVelocity.x * 2;
            pushBack = Mathf.Sign(pushBack) * Mathf.Max(30, Mathf.Abs(pushBack));
            other.transform.GetComponent<playerController>().bounceDirection += pushBack * Vector2.right + Vector2.up * 10;
            bounceDirection.x = 0;
            rigid.velocity = new Vector2(0, -maxSmashSpeed);
            return;
        }

        audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 1, UnityEngine.Random.Range(0.96f, 1.03f));

        Vector2 dir = Vector2.zero;

        bool onTop = false;
        if (centerOfGravity == null) {
            onTop = other.transform.position.y + downLazy / 1.25f < this.transform.position.y - downLazy/ 1.25f;
        } else { 
            onTop = Vector3.Distance(centerOfGravity.position, transform.position + downLazy / 1.5f * transform.up) < Vector3.Distance(other.transform.GetComponent<playerController>().centerOfGravity.position, other.transform.position - transform.GetComponent<playerController>().downLazy / 1.5f * other.transform.up);
        }
        float aboveMultiplyer = (onTop) ? (instantBounceKill ? 20 : 10) : 0;

        dir.y = Mathf.Clamp(transform.InverseTransformDirection(other.relativeVelocity).y, aboveMultiplyer, 50);
        dir.y *= (smashing ? 1 : 1.5f) * (chargeValue > 0.1f ? 1.25f : 1);
        dir.y = Mathf.Min(dir.y, 30);
        if (onTop) {
            dir.y = Mathf.Max(dir.y, 25);
        }

        dir.x = transform.InverseTransformDirection(other.relativeVelocity).x * (smashing ? 0.2f : 1);
        if (!touchingGround) {
            dir.x *= 1.25f;
        }
        dir.x = Mathf.Min(Mathf.Abs(dir.x), 32) * Mathf.Sign(dir.x);

        if (onTop) {
            colParticle.GetComponent<ParticleSystem>().startSize = 1.5f;
            colParticle.GetComponent<ParticleSystem>().startSpeed = 70f;
            StartCoroutine(headBoopSquish());
            StartCoroutine(other.transform.GetComponent<playerController>().headBoopSquish());
            audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 1, UnityEngine.Random.Range(0.96f, 1.03f));
        }

        print(dir);

        bounceDirection += dir;
    }

    //checks neighboring wave segments and allows player to walk up slopes to some extent
    protected void slopeCheck() {
        float littleHeight = 0.15f;
        float height = -1;

        for (int i = 1; i < 10; i++) {
            Debug.DrawRay(transform.position - transform.up * 0.66f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x) * 0.7f, Color.red);
            RaycastHit2D slopeDetect = Physics2D.Raycast(transform.position - transform.up * 0.6f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x), 0.7f, groundCheck);
            if (slopeDetect) {
                height = i;
            }
        }
        

        if (height > -1 && height < 9) {
            checkGround();
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + littleHeight * height + 0.4f), Time.deltaTime * 12);
        }
    }

    IEnumerator recovery(float recoveryTime) {
        smashing = false;
        canSmash = false;
        

        vunrabilityFrames = true;

        yield return new WaitForSeconds(recoveryTime);

        vunrabilityFrames = false;

        yield return new WaitForSeconds(recoveryTime);

        GameObject rechargeCircle = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
        rechargeCircle.transform.SetParent(transform);
        rechargeCircle.GetComponent<SpriteRenderer>().color = changeOpacity(spriteAnim.GetComponent<SpriteRenderer>().color, rechargeCircle.GetComponent<SpriteRenderer>().color.a);
        spriteAnim.GetComponent<SpriteRenderer>().color = changeOpacity(spriteAnim.GetComponent<SpriteRenderer>().color, 225f);
        audioManager.instance.Play(loadPower, 0.5f, UnityEngine.Random.Range(0.96f, 1.03f));
        SmashCooldownTime = 0.25f;
        canSmash = true;
    }

    protected bool alreadyDead;

    public virtual void die() {
        if (!endingUI.instance.endConditionMet) {
            alreadyDead = true;
            audioManager.instance.Play(deathExplosion, 0.5f, UnityEngine.Random.Range(0.96f, 1.04f));

            GameObject particle = Instantiate(deathParticle, transform.position, Quaternion.identity) as GameObject;
            particle.GetComponent<ParticleSystem>().startColor = Color.Lerp(baseColor, Color.white, 0.5f);

            GetComponent<SpriteRenderer>().color = fullColor;
            Shake.instance.shake(2, 3);

            endingUI.instance.StopAllCoroutines();
            endingUI.instance.StartCoroutine(endingUI.instance.checkPlayersLeft());

            Destroy(this.gameObject);
        }

    }

    void SpawnTrail() {
        GameObject trailPart = new GameObject();
        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
        trailPartRenderer.sprite = spriteAnim.GetComponent<SpriteRenderer>().sprite;
        trailPartRenderer.flipX = spriteAnim.GetComponent<SpriteRenderer>().flipX;

        trailPartRenderer.color = changeOpacity(fullColor, 0.25f);

        trailPart.transform.position = transform.position;
        trailPart.transform.localScale = transform.localScale;
        Destroy(trailPart, 2f); // replace 0.5f with needed lifeTime

        StartCoroutine("FadeTrailPart", trailPartRenderer);
    }

    public IEnumerator headBoopSquish() {
        StopCoroutine("squishControl");
        float t = 1f;
        while (sqetch.animatedStretch < 1.5f && t > 0) {
            sqetch.animatedStretch += 0.5f;
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while (sqetch.animatedStretch > 0.1f && t > 0) {
            sqetch.animatedStretch -= 0.55f;
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        sqetch.animatedStretch = 0;

    }

    public IEnumerator squishControl(float squatchAmount, float speed) {
        if (sqetch.animatedStretch < squatchAmount) {
            float t = 1f;
            while (sqetch.animatedStretch < squatchAmount){
                sqetch.animatedStretch += speed;
                yield return new WaitForEndOfFrame();
            }
        } else {
            while (sqetch.animatedStretch >squatchAmount)
            {
                sqetch.animatedStretch -= speed;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer) {
        while (trailPartRenderer != null && trailPartRenderer.color.a > 0) {
            trailPartRenderer.color = changeOpacity(trailPartRenderer.color, trailPartRenderer.color.a - 0.01f);
            //trailPartRenderer.transform.localScale = trailPartRenderer.transform.localScale - Vector3.one * 0.2f;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator checkIfWaved() {
        canMakeWave = false;
        yield return new WaitForSeconds(1.5f); 
        canMakeWave = true;
    }

    public Color changeOpacity(Color color, float opacity) {
        color.a = opacity;
        return color;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(transform.position + Vector3.right * transform.GetComponent<playerController>().topWidth / 2, new Vector2(0.1f,1));
        //Gizmos.DrawWireCube(transform.position - Vector3.right * transform.GetComponent<playerController>().topWidth / 2, new Vector2(0.1f, 1));
    }

    [Command] //dumb ugly internet stuffs
    public void CmdMakeWave(Vector2 position, float amplitude, Color color, float velocity) {
        GameObject Pulse = Instantiate(pulse, new Vector3(position.x, position.y, 0), Quaternion.identity);

        Pulse.GetComponent<PulseMove>().Amplitude = amplitude;
        Pulse.GetComponent<PulseMove>().color = color;
        Pulse.GetComponent<PulseMove>().speed = velocity;

        NetworkServer.Spawn(Pulse);

        GameObject AntiPulse = Instantiate(antiPulse, new Vector3(position.x, position.y, 0), Quaternion.identity);
        AntiPulse.GetComponent<AntiPulseMove>().Amplitude = amplitude;
        AntiPulse.GetComponent<AntiPulseMove>().color = color;
        AntiPulse.GetComponent<AntiPulseMove>().speed = velocity;

        NetworkServer.Spawn(AntiPulse);
    }

    public int CompareTo(playerController other) {
        return GameManager.instance.playerScores[playerNum].CompareTo(GameManager.instance.playerScores[other.playerNum]);
    }
}

