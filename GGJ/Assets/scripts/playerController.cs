using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

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

    protected float maxSmashVulnerabilityTime;
    [SyncVar] protected bool smashing;
    protected float SmashCooldownTime = 0;
    public float waveSpeed = 7;
    public float maxChargeTime = 0.75f;
    protected bool vunrabilityFrames = false;

    protected float previousAmplitude = 0;
    bool canMakeWave = true;
    [SyncVar]  protected bool canSmash = true;

    [Header("Dash Properties")]
    public float dashSpeed = 7;
    public float dashCharge = 0.35f;
    bool canDash = true;


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

    [Header("Bounciness")]
    public float bounciness;
    [SyncVar] [HideInInspector] public Vector2 bounceDirection;
     
    private SquashAndStretch sqetch;
    public float slidingFloor;

    [Header("Modifiers")]
    public bool airControl;
    public bool seperateDashCooldown;
    public bool canDashOnGround;
    public bool spikeyHats;
    public bool fullChargeInvinc;
    public bool holdMaxSmash;
    [Space()]
    //online stuffs
    public bool onlinePlayer;
    public GameObject pulse;
    public GameObject antiPulse;
    public onlineClient client;

    protected void Start() {

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        changeModifiers();

        anim.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

        baseColor = GetComponent<SpriteRenderer>().color;
        sqetch = GetComponent<SquashAndStretch>();
        chargeParticle.startColor = Color.Lerp(baseColor, Color.white, 0.5f);

        chargeVisualEffects(0);
        chargeCircle.transform.localScale = Vector3.zero;
    }

    void changeModifiers() {
        //Debug bounciness and charge
        airControl = GameManager.instance.airControl;
        seperateDashCooldown = GameManager.instance.seperateDashCooldown;
        canDashOnGround = GameManager.instance.canDashOnGround;
        maxSmashPower = GameManager.instance.maxSmashPower;
        bounceForce = GameManager.instance.bounciness;
        maxSmashSpeed = GameManager.instance.maxSmashSpeed;
        fullChargeInvinc = GameManager.instance.fullChargeInvinc;
        holdMaxSmash = GameManager.instance.holdMaxSmash;
        maxChargeTime = GameManager.instance.maxChargeTime;
    }

    void LateUpdate() {
        if (onlinePlayer) {
            if (client != null) {
                CmdinputAudit(client.HorizInput);
            } else {
                return;
            }
        }

        float HorizInput = Input.GetAxis("Horizontal" + playerControl);
        //CmdinputAudit(HorizInput);
    }

    public void CmdinputAudit(float HorizInput) {
        anim.SetFloat("velocity", Mathf.Abs(HorizInput));

        checkGround();
        bounceDirection = new Vector2(Mathf.Lerp(bounceDirection.x, 0, Time.deltaTime * (smashing ? 25 : 2.5f)), Mathf.Lerp(bounceDirection.y, 0, Time.deltaTime * (smashing ? 5 : 35)));

        if (!smashing && !vunrabilityFrames && playerControl != "" && rigid.bodyType == RigidbodyType2D.Dynamic) {
            movement(HorizInput);
        } if (!smashing && !vunrabilityFrames) {
            rigid.velocity = new Vector2(xSpeed, rigid.velocity.y);
        }

        //chargeVisualEffects(lifeTimeDebug);
        rigid.velocity += gravityDirection * gravityStrength + bounceDirection;
    }

    [HideInInspector] public bool jumped;
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
                checkGround();
                Invoke("jumpDelay", 0.05f);
            }

            //dashingOnGround
            if (canDash && Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && canDashOnGround) {
                StartCoroutine(dashOutOfCharge(Input.GetAxis("Dash" + playerControl), true, true));
            }
        } else {
            if (canSmash) {
                if (canDash && Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f) {
                    StartCoroutine(dashOutOfCharge(Input.GetAxis("Dash" + playerControl), true, false));

                    //float dir = Input.GetAxisRaw("Horizontal" + playerControl);
                    //if (dir == 0)
                    //    dir = 1;

                    //StartCoroutine(dashOutOfCharge(dir, true, false));
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
        anim.GetComponent<SpriteRenderer>().color = Color.Lerp(baseColor, fullColor, toggle);
        chargeParticle.startSize = Mathf.Lerp(0.25f, 0.75f, toggle * 10);
        float changeSize = Mathf.Lerp(0, 1, toggle * 2);
        chargeCircle.transform.localScale = Vector3.one * Mathf.Lerp(0.27f,0.1f, toggle);

        Color chargeColor= GetComponent<SpriteRenderer>().color;
        chargeColor.a = Mathf.Lerp(0,0.5f,toggle);
        chargeCircle.GetComponent<SpriteRenderer>().color = chargeColor;

        chargeParticle.transform.localScale = new Vector3(changeSize, changeSize, changeSize);
    }

    float chargeValue;
    protected IEnumerator chargeSmash(float currentDirection) {
        bounceDirection.x = bounceDirection.x / 12;
        //bounceDirection.y = Mathf.Max(0,bounceDirection.y - 3);
        //rigid.velocity =  Vector2.right * rigid.velocity.x + bounceDirection;

        smashing = true;

        bounceDirection.y *= 10;
        // print(bounceDirection.y);

        bool direction = GetComponent<SpriteRenderer>().flipX;
        bool maxed = false;
        chargeValue = 0;
        SmashSpeed = minSmashSpeed;
        smashPower = minSmashPower;
        float hold = maxChargeTime + (holdMaxSmash ? 0.5f : 0);

        float initialY = Mathf.Max(0, rigid.velocity.y);

        while (chargeValue <= hold) {
        //while (1 == 1) {   
            float lerp = (chargeValue / maxChargeTime);
            chargeVisualEffects(lerp);

            SmashSpeed = Mathf.Lerp(minSmashSpeed, maxSmashSpeed, lerp);
            smashPower = Mathf.Lerp(minSmashPower, maxSmashPower, lerp);
            SmashCooldownTime = Mathf.Lerp(0.25f, maxSmashVulnerabilityTime, lerp);

            if (!Input.GetButton("Smash" + playerControl) || smashing == false) {
                audioManager.instance.Play(charge, 0.5f * (chargeValue / maxChargeTime));
                anim.SetBool("smashing", false);
                break;
            }

            //Dash out of charge
            if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && Input.GetAxis("Dash" + playerControl) != currentDirection) {
                chargeValue = 0;
                chargeCircle.transform.localScale = Vector3.zero;
                StartCoroutine(dashOutOfCharge(chargeValue, direction, false));
                yield break;
            }

            currentDirection = Input.GetAxis("Dash" + playerControl);
            chargeValue += Time.deltaTime;

            if (chargeValue >= maxChargeTime && !maxed) {
                audioManager.instance.Play(charge, 0.5f * (chargeValue / maxChargeTime));

                GameObject wave = Instantiate(shockWave, transform.position, transform.rotation) as GameObject;
                wave.transform.SetParent(transform);
                wave.GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, wave.GetComponent<SpriteRenderer>().color.a);

                Color color = GetComponent<SpriteRenderer>().color;
                color.a = 0.5f;
                anim.GetComponent<SpriteRenderer>().color = color;
                color = GetComponent<SpriteRenderer>().color;
                color.a = 255f;
                anim.GetComponent<SpriteRenderer>().color = color;

                maxed = true;
            }

            rigid.velocity = (Vector2.right * rigid.velocity.x) * 0.95f + Vector2.up * initialY +  bounceDirection;

            initialY /= 1.1f;

            if(airControl)
                rigid.velocity  += Vector2.right * Input.GetAxis("Horizontal" + playerControl) * 0.3f;

            yield return new WaitForEndOfFrame();
        }

        chargeValue = Mathf.Min(chargeValue, maxChargeTime);
        StartCoroutine(smashAfterCharge(chargeValue));
    }

    protected IEnumerator dashOutOfCharge(float chargeValue, bool direction) {
        yield return dashOutOfCharge(chargeValue, direction, false);
        chargeCircle.transform.localScale = Vector3.zero;
        chargeValue = 0;
    }

    protected IEnumerator dashOutOfCharge(float chargeValue, bool direction, bool onGround) {
        InvokeRepeating("SpawnTrail", 0, 0.035f);
        GetComponent<SpriteRenderer>().flipX = direction;
        bounceDirection += new Vector2(dashSpeed * Input.GetAxisRaw("Dash" + playerControl) / (onGround ? 1.5f : 1), 0);
        //bounceDirection += new Vector2(dashSpeed * Input.GetAxisRaw("Horizontal" + playerControl) / (onGround ? 1.5f : 1), 0);

        rigid.velocity = new Vector2(0, rigid.velocity.y);


        smashing = false;
        chargeVisualEffects(0);
        SmashSpeed = 0;
        smashPower = 0;

        audioManager.instance.Play(this.cancel, 0.25f * (chargeValue / maxChargeTime));
        GameObject dashExpurosion = Instantiate(dashCancelParticle, transform.position, transform.rotation);
        dashExpurosion.GetComponent<ParticleSystem>().startColor = fullColor;
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
            //StartCoroutine(recovery(0.3f));
            yield return new WaitForSeconds(0.2f);
            canSmash = true;
            chargeParticle.gameObject.transform.localScale = new Vector3(rigid.velocity.x, 0);
            CancelInvoke("SpawnTrail");
            yield return new WaitForSeconds(1f);
            canDash = true;
        }

    }

    protected IEnumerator smashAfterCharge(float chargeValue) {
        if (chargeValue > maxChargeTime / 2)
            InvokeRepeating("SpawnTrail", 0, 0.035f);
        yield return new WaitForSeconds(0.05f);
        chargeParticle.gameObject.transform.localScale = Vector3.zero;
        rigid.velocity = new Vector2(0, -SmashSpeed);
        bounceDirection.y /= 2;
        anim.SetBool("smashing", true);

        createDashParticle(1.5f);
        
        chargeVisualEffects(0);
        chargeCircle.transform.localScale = Vector3.zero;
    }

    public void createDashParticle(float lifetime) {
        GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
        dashTrail.transform.parent = this.transform;
        dashTrail.GetComponent<ParticleSystem>().startColor = baseColor *  new Vector4(1,1,1,0.75f);
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
                    if (square.GetComponent<SquareBehavior>().TotalAmplitude - previousAmplitude > 0.1f) {
                        bounceDirection += Vector2.up * square.GetComponent<SquareBehavior>().TotalAmplitude;
                        //bounceDirection.y *= bounceForce / (square.transform.position.y < square.GetComponent<SquareBehavior>().initialY ? 15 : 4);
                        bounceDirection.y *= bounceForce / (jumped ? 3 : 1);
                        grounded = false;                    
                    }
                    previousAmplitude = square.GetComponent<SquareBehavior>().TotalAmplitude; // have the swuare itself keep track of its own acceleration or something
                }
            } 
        }
        
        anim.SetBool("airborne", !grounded);
        if (!canMakeWave) {
            StartCoroutine("checkIfWaved");
        }
        return grounded;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor"))
        {

            float strength = Mathf.Clamp(other.relativeVelocity.magnitude / 40f, 0, .8f);
            if (smashing)
            {
                StopCoroutine("chargeSmash");
                StopCoroutine("smashAfterCharge");
                CancelInvoke("SpawnTrail");
                canSmash = false;

                if (Shake.instance != null)
                    Shake.instance.shake(2, 3);

                chargeValue = 0;
                rigid.velocity = Vector3.zero;
                strength *= smashPower;
                Color color = GetComponent<SpriteRenderer>().color;
                color.a = 0.75f;

                if (onlinePlayer)
                {
                    CmdMakeWave(transform.position + Vector3.up * -1, strength, color, waveSpeed);
                }
                else
                {
                    WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength, color, waveSpeed, null);
                }

                audioManager.instance.Play(smash, 0.75f, UnityEngine.Random.Range(0.95f, 1.05f));
                SmashSpeed = 0;
                smashPower = 0;
                StartCoroutine(recovery(SmashCooldownTime));
            }
            else
            {
                if (other.relativeVelocity.magnitude > 12 && !onlinePlayer)
                {
                    audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.05f, UnityEngine.Random.Range(0.96f, 1.03f));
                    if (canMakeWave)
                        if (onlinePlayer)
                        {
                            CmdMakeWave(transform.position + Vector3.up * -1, strength / 2, Color.white, 1.5f);
                        }
                        else
                        {
                            WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength / 2, Color.white, 1.5f, null);
                        }
                }
            }

        }
        else if (other.gameObject.tag.Equals("Player"))
        {

            GameObject colParticle = Instantiate(collisionParticle, other.contacts[0].point, transform.rotation);
            colParticle.GetComponent<ParticleSystem>().startColor = fullColor;
            Destroy(colParticle, 0.75f);

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
            dir.x = other.relativeVelocity.x * (smashing ? 0.05f : 1);
            if (!checkGround())
            {
                dir.x *= 1.25f;
            }
            dir.x = Mathf.Min(Mathf.Abs(dir.x), 50) * Mathf.Sign(dir.x);

            float aboveMultiplyer = (other.transform.position.y - downLazy < this.transform.position.y) ? 5f : -1;
            dir.y = Mathf.Clamp(other.relativeVelocity.y, aboveMultiplyer, 50);
            dir.y *= (smashing ? 1 : 1.5f) * (chargeValue > 0.1f ? 2 : 1);

            bounceDirection += dir * bounciness;

            if (bounceDirection.y < 0)
            {
                StartCoroutine("headBoopSquish");
            }
        }
        else if (other.gameObject.tag.Equals("Spike") && !alreadyDead)
        {
            die();
        }
        else { //bounce off side walls
            bounceDirection.x *= -0.75f;
        }
    }

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
            
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + littleHeight * height + 0.4f), Time.deltaTime * 12);
            //checkGround(); //THIS MAY OR MAY NOT BE PART O THE PROBLEM. ONCE I STANDARDIZE WAVES WE MAY BE ABLE TO TAKE THIS OUT
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
        particle.GetComponent<ParticleSystem>().startColor = Color.Lerp(baseColor, Color.white, 0.5f);
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
        targetColor.a = 0.25f;
        trailPartRenderer.color = targetColor;

        trailPart.transform.position = transform.position;
        trailPart.transform.localScale = transform.localScale;
        Destroy(trailPart, 2f); // replace 0.5f with needed lifeTime

        StartCoroutine("FadeTrailPart", trailPartRenderer);
    }

    IEnumerator headBoopSquish() {
        sqetch.animatedStretch = 1.5f;
        while (sqetch.animatedStretch > 0.1f) {
            sqetch.animatedStretch -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
        sqetch.animatedStretch = 0;

    }

    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer) {
        while (trailPartRenderer != null && trailPartRenderer.color.a > 0) {
            Color color = trailPartRenderer.color;
            color.a -= 0.01f; // replace 0.5f with needed alpha decrement
            trailPartRenderer.color = color;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator checkIfWaved() {
        canMakeWave = false;
        yield return new WaitForSeconds(1.5f); 
        canMakeWave = true;
    }

    [Command]
    public void CmdMakeWave(Vector2 position, float amplitude, Color color, float velocity)
    {
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
}

