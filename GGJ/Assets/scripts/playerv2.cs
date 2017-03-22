using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerv2 : MonoBehaviour {

    Rigidbody2D rigid;
    private Animator anim;
    public LayerMask groundCheck;
    public Transform centerOfGravity;
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

    public bool smashing;
    float smashReset;

    public float maxSmashCooldownTime = 2;
    float SmashCooldownTime = 0;

    private float previousAmplitude = 0;
    public float maxChargeTime = 0.75f;

    bool canMakeWave = true;

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
    public GameObject deathParticle;
    public AudioClip deathExplosion;

    [Header("Sound Effects")]
    public AudioClip[] softLanding;
    public AudioClip loadPower;
    public AudioClip smash;
    public AudioClip jump;
    public AudioClip charge;
    public AudioClip cancel;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        anim.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        baseColor = GetComponent<SpriteRenderer>().color;

        toggleCharge(0);
        if (controllerHandler.controlOrder.Count > playerNum)
        {
            playerControl = controllerHandler.controlOrder[playerNum];
        }
    }

    void toggleCharge(float toggle) {
        GetComponent<SpriteRenderer>().color = Color.Lerp(baseColor, fullColor, toggle);
        chargeParticle.startSize = Mathf.Lerp(0, 0.4f, toggle);
        float changeSize = Mathf.Lerp(0, 0.75f, toggle);
        chargeParticle.transform.localScale = new Vector3(changeSize, changeSize, changeSize);
        speed = Mathf.Lerp(speed, 0, toggle / 24);
    }

    void Update() {
        anim.SetFloat("velocity", 0);
        bool touchingGround = checkGround();
        setGravity();
        if (!smashing && !vunrabilityFrames && playerControl != "") {
            if (Mathf.Abs(Input.GetAxis("Horizontal" + playerControl)) > 0.1f) {

                xSpeed += speed * Input.GetAxis("Horizontal" + playerControl) / (accelerate * 4);
                anim.GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal" + playerControl) < 0;
                anim.SetFloat("velocity", 1);
                slopeCheck();
            } else {
                xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * decelerate);
            }

            xSpeed = Mathf.Clamp(xSpeed, -speed, speed);

            if (Input.GetButtonDown("Jump" + playerControl) && touchingGround) {
                //rigid.velocity = new Vector2(rigid.velocity.x, maxJumpHeight);
                rigid.velocity = (Vector2)transform.up * maxJumpHeight; 
                audioManager.instance.Play(jump, 0.5f, UnityEngine.Random.Range(0.97f, 1.03f));
            }

            if (Input.GetButtonUp("Jump" + playerControl) && rigid.velocity.y > minJumpHeight)  {
                //rigid.velocity = new Vector2(rigid.velocity.x, minJumpHeight);
                rigid.velocity = (Vector2)transform.up * -minJumpHeight; 
            }

            if (Input.GetButtonDown("Smash" + playerControl)
                    && canSmash && !smashing && !touchingGround) {
                StartCoroutine(chargeSmash(Input.GetAxis("Horizontal" + playerControl)));
            }

            rigid.velocity = transform.TransformDirection(new Vector3(xSpeed, transform.InverseTransformDirection(rigid.velocity).y));
            Debug.DrawRay(transform.position, transform.right * xSpeed, Color.green);
            //rigid.velocity = (Vector3)rigid.velocity + transform.right * xSpeed;
        }
    }

    void setGravity() {
        Vector2 dirOfGravity = (-transform.position + centerOfGravity.position).normalized;

        rigid.AddForce(dirOfGravity * 10);

        Vector2 dirUp = -dirOfGravity;
        this.transform.up = Vector2.Lerp(this.transform.up, dirUp, Time.deltaTime * 500);
        //Quaternion targetRotation = Quaternion.FromToRotation(dirUp, transform.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7) * transform.rotation;
        //Debug.DrawRay(transform.position, dirOfGravity * 10, Color.green);
    }

    IEnumerator chargeSmash(float currentDirection) {
        smashing = true;
        bool direction = GetComponent<SpriteRenderer>().flipX;
        float chargeValue = 0;

        float originalSpeed = speed;
        speed /= 3;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;

        SmashSpeed = minSmashSpeed;
        smashPower = minSmashPower;

        while (chargeValue < maxChargeTime) {
            float lerp = (chargeValue / maxChargeTime);
            toggleCharge(lerp);
            SmashSpeed = Mathf.Lerp(minSmashSpeed, maxSmashSpeed, lerp);
            smashPower = Mathf.Lerp(minSmashPower, maxSmashPower, lerp);
            SmashCooldownTime = Mathf.Lerp(0.25f, maxSmashCooldownTime, lerp);


            if (!Input.GetButton("Smash" + playerControl)) {
                speed = originalSpeed;
                StartCoroutine(smashAfterCharge(chargeValue));
                yield break;
            }

            //Dash out of charge
            if (Mathf.Abs(Input.GetAxis("Dash" + playerControl)) > 0.5f && Input.GetAxis("Dash" + playerControl) != currentDirection)  {
                speed = originalSpeed;
                StartCoroutine(dashOutOfCharge(chargeValue, direction));
                yield break;
            }
            chargeValue += Time.deltaTime;
            rigid.velocity = Vector2.right * speed * currentDirection;
            yield return new WaitForEndOfFrame();
        }
        speed = originalSpeed;
        StartCoroutine(smashAfterCharge(chargeValue));
    }

    IEnumerator dashOutOfCharge(float chargeValue, bool direction) {
        vunrabilityFrames = true;
        GetComponent<SpriteRenderer>().flipX = direction;
        //rigid.velocity = new Vector2(dashSpeed * Input.GetAxis("Dash" + playerControl), 0);
        rigid.velocity = transform.TransformDirection(new Vector3(dashSpeed * Input.GetAxis("Dash" + playerControl), 0));

        smashing = false;
        toggleCharge(0);

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
        Destroy(dashTrail, 2f);

        //canSmash = false;
        StartCoroutine(recovery(0.5f));
        yield return new WaitForSeconds(0.15f);
        chargeParticle.gameObject.transform.localScale = new Vector3(rigid.velocity.x, 0);
        vunrabilityFrames = false;
    }

    IEnumerator smashAfterCharge(float chargeValue) {
        audioManager.instance.Play(charge, 0.5f * (chargeValue / maxChargeTime));
        yield return new WaitForSeconds(0.05f);
        chargeParticle.gameObject.transform.localScale = Vector3.zero;
        //rigid.velocity = new Vector2(0, -SmashSpeed);
        rigid.velocity = transform.TransformDirection(new Vector3(0, -SmashSpeed));

        anim.SetBool("smashing", true);
        smashReset = 50;

        GameObject dashTrail = Instantiate(dashTrailParticle, transform.position, transform.rotation);
        dashTrail.transform.parent = this.transform;
        dashTrail.GetComponent<ParticleSystem>().startColor = fullColor;
        Destroy(dashTrail, 1.5f);
        toggleCharge(0);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Floor"))
        {
            if (other.relativeVelocity.magnitude > 8)
            {
                float strength = Mathf.Clamp(other.relativeVelocity.magnitude / 40f, 0, .8f);
                if (smashing)
                {
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
                    StartCoroutine(recovery(SmashCooldownTime));
                } else {
                    //rigid.velocity = Vector3.zero;
                    audioManager.instance.Play(softLanding[UnityEngine.Random.Range(0, softLanding.Length - 1)], 0.05f, UnityEngine.Random.Range(0.96f, 1.03f));

                    if (canMakeWave)
                        WaveGenerator.instance.makeWave(transform.position + Vector3.up * -1, strength, Color.white, 3);
                }
            }
        }
        else if (other.gameObject.tag.Equals("Player"))
        {
            if (other.gameObject.transform.position.y < transform.position.y && !checkGround())
                rigid.velocity = new Vector2(rigid.velocity.x, (smashing) ? 15 : 5);
        }
        else if (other.gameObject.tag.Equals("Spike"))
        {
            die();
        }
    }

    void checkForWave()
    {

        foreach (GameObject square in GameObject.FindGameObjectsWithTag("Floor")) {
            if (Mathf.Abs(square.transform.position.x - transform.position.x) < GameObject.Find("Managers").GetComponent<GameManager>().Square.transform.localScale.x) {
                if (square.GetComponent<SquareBehavior>() != null && square.GetComponent<SquareBehavior>().TotalAmplitude - previousAmplitude > .1) {
                    rigid.AddForce(new Vector2(0, square.GetComponent<SquareBehavior>().TotalAmplitude * bounceForce));
                }

                if(square.GetComponent<SquareBehavior>() != null)
                    previousAmplitude = square.GetComponent<SquareBehavior>().TotalAmplitude;
            }
        }

        if (!canMakeWave)
        {
            return;
        }

        StartCoroutine("checkIfWaved");
    }

    IEnumerator checkIfWaved()
    {
        canMakeWave = false;
        yield return new WaitForSeconds(0.75f);
        canMakeWave = true;
    }

    void OnCollisionExit2D(Collision2D other)
    {
        checkForWave();
    }

    void slopeCheck()
    {
        float littleHeight = 0.05f;
        float height = -1;

        for (int i = 1; i < 3; i++)
        {
            Debug.DrawRay(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x) * 0.6f, Color.red);
            RaycastHit2D slopeDetect = Physics2D.Raycast(transform.position - transform.up * 0.33f + transform.up * littleHeight * i, transform.right * Mathf.Sign(rigid.velocity.x), 0.6f, groundCheck);
            if (slopeDetect.collider != null)
            {
                height = i;
            }
        }
        if (height != 2 && height > -1)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y + littleHeight * height + 0.2f), Time.deltaTime * 4);
        }
    }

    IEnumerator recovery(float recoveryTime)
    {
        smashing = false;
        canSmash = false;
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0.5f;
        anim.GetComponent<SpriteRenderer>().color = color;
        vunrabilityFrames = true;

        yield return new WaitForSeconds(1);

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

    public bool checkGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(GetComponent<Collider2D>().bounds.min, -transform.up, 0.5f, groundCheck);
        Debug.DrawLine(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.min - transform.up * 0.5f);
        anim.SetBool("airborne", hit == false);
        return hit;
    }

    public void die()
    {
        audioManager.instance.Play(deathExplosion, 0.5f, UnityEngine.Random.Range(0.96f, 1.04f));
        GameObject particle = Instantiate(deathParticle, transform.position, Quaternion.identity) as GameObject;
        particle.GetComponent<ParticleSystem>().startColor = fullColor;
        GetComponent<SpriteRenderer>().color = fullColor;
        Shake.instance.shake(2, 3);
        endingUI.instance.Invoke("checkPlayersLeft", 0.5f);
        Destroy(this.gameObject);
    }
}
