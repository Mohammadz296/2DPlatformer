using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movementscript : CharacterMovement
{
    [SerializeField] float speed;
    [SerializeField] float airSpeed;
    [SerializeField] float wallG;
    [SerializeField] float rollTime;
    [SerializeField] float kayoteTime;
    [SerializeField] float acceleration;
    [SerializeField] float jumpBufferTime;
    [SerializeField] float wallJumpTimer;
    [SerializeField] float staminaWaitTime;
    [SerializeField] float minSpeed;
    [SerializeField] float dashTimer;
    [SerializeField] float slopeSpeed;
    [SerializeField] float trailSpawnRate;

    [SerializeField] GameObject trail;
    [SerializeField] Vector2 wallJumpPower;
    [SerializeField] Vector2 dashForce;

    public bool isRolling { get; private set; }
    [HideInInspector] public float finalStamina;
   public bool deflected { get; private set; }


    
    PlayerStats playerPrefs;

    CapsuleCollider2D bc;
    CircleCollider2D bc2;
    EnvironmentManager em;

    Transform rollCheck;
 


    RoofCheck[] roofChecks = new RoofCheck[8];

    bool _jump;
    bool _shift;
    bool _slope;
    bool _wall;
    bool _s;
    bool _roll;
    bool _notJump;


    bool wallJump;
    bool kayote;
    bool isDashing;
    bool isSliding;
  
    bool waitStamina = true;
    bool canRoof = true;
    
    bool canSpawnTrail = true;

    [field: SerializeField] public float stamina { get; private set; }
    [SerializeField] float rollForce;

    float rollTemp;
    float jumpDir;

    float vertical;
    float kayoteTimeTemp;
    float dashTimeTemp;
    float drag;
    float timerTemp;


    Vector2 slopeNormalPerp;
    List<InputBuffer> inputs = new();

    State status;
    InputBuffer inputBuff;

    enum State
    {
        idle,
        jumping,
        walking,
        falling,
        dashing,
        wallslide,
        rolling,
        sloping,
        sliding,
        immobilized,

    }
    enum InputBuffer
    {
        none,
        jump,
        roll,
        dash,
    }
    void Start()
    {
        SetStart();
    }
    void Update()
    {
        if (!playerPrefs.isDead)
        {
                     
            if (canSpawnTrail)
                StartCoroutine(SpawnTrail());
            ReadInput();
        }
    }
    private void FixedUpdate()
    {
        if (!playerPrefs.isDead)
        {
            switch (status)
            {
                case State.jumping:

                    if (canJump)
                    {
                        RemoveInputBuff();              
                        Jump();
                    }
                    if (canRoof)
                        RoofCheck(0);

                    RunAir();
                    if (rb.velocity.y > 0 && rb.velocity.y <= maxMaxThreshold && canMax)
                        MaxHeight();

                    if (_ground && rb.velocity.y == 0)                   
                        CheckPosition();
                    break;
                case State.walking:
                    if (!animator.GetBool("IdleBlock") && playerPrefs.canParry)
                        Run();
                    else if (playerPrefs.canParry)
                        Run(0f);
                    else
                        rb.velocity = Vector2.zero;
                    if (!playerPrefs.click1)
                        Stamina(30f);
                    CheckPosition();
                    break;
                case State.sloping:
                    RunSlope();
                    Stamina(10f);
                    CheckPosition();
                    break;
                case State.wallslide:
                    if (!wallJump)
                        WallSlide();
                    if (inputBuff == InputBuffer.jump && _wall && !wallJump)
                    {
                        jumpDir = -facing;
                        StartCoroutine(WallJump());
                    }
                    if (wallJump)
                        RunAir(0);

                    else
                        RunAir();
                    CheckPosition();

                    break;
                case State.idle:
                    Idle();
                    if (!playerPrefs.click1)
                        Stamina(80f);
                    CheckPosition();
                    break;
                case State.rolling:


                    RunAir();

                    if (_roll)
                        rollTemp = .1f;

                    if (!isRolling)
                        Roll();

                    else if (inputBuff == InputBuffer.jump && _ground)
                    {
                        StopRolling();
                        status = State.jumping;
                    }
                    else if (isRolling && rollTemp <= 0)
                    {
                        StopRolling();
                        CheckPosition();
                    }

                    else if (isRolling && !_roll)
                        rollTemp = (rollTemp - 1 * Time.deltaTime);

                    break;
                case State.falling:
                    if (canRoof)
                        RoofCheck(4);
                    if (inputBuff==InputBuffer.jump && kayote && canJump)
                    {
                        Jump();
                        status = State.jumping;
                    }
                    else if (_ground)
                        CheckPosition();

                    else
                    {
                        Falling();
                        RunAir();
                    }
                    break;
                case State.dashing:

                    RunAir(.5f);
                    if (_wall)
                        rb.velocity = Vector2.zero;
                    if (!isDashing)
                        Dash();
                    if (dashTimeTemp > 0)
                    {
                        kayoteTimeTemp = 0;
                        dashTimeTemp = dashTimeTemp - 1 * Time.deltaTime;
                    }
                    else
                    {
                        rb.gravityScale = gravity;
                        isDashing = false;
                        CheckPosition();
                    }
                    break;
                case State.sliding:
                    Stamina(40f);
                    if (_slope)
                        Sliding();
                    else if (!_slope)
                        CheckPosition();
                    if (inputBuff == InputBuffer.jump && _slope)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, 0);
                        canJump = true;
                        status = State.jumping;
                    }
                    break;
                case State.immobilized:
                    break;

            }
        }
    }
    void SetStart()
    {
        _transform = transform;
        animator =_transform.Find("character").GetComponent<Animator>();
        groundCheck = _transform.Find("groundCheck").GetComponent<EnvironmentCheck>();
        rollCheck = _transform.Find("rollCheck").transform;
        wallCheck = _transform.Find("wallCheck").GetComponent<EnvironmentCheck>(); ;
        wallCheck2 = _transform.Find("wallCheck2").GetComponent<EnvironmentCheck>(); ;
        em = _transform.GetComponentInChildren<EnvironmentManager>();
        skin = _transform.Find("character").GetComponent<CharacterEvent>();
        bc = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerPrefs = GetComponent<PlayerStats>();
        bc2 = GetComponent<CircleCollider2D>();
        facing = skin.facing;

        groundCheck.canWalk = canWalk;
        wallCheck.canWalk=canWalk;
        wallCheck2.canWalk=canWalk;
        rollTemp = rollTime;
        finalStamina = stamina;
        drag = rb.drag;
        gravity = rb.gravityScale;
        kayoteTimeTemp = kayoteTime;
        timerTemp = staminaWaitTime;

        for (int i = 0; i < roofChecks.Length; i++)
            roofChecks[i] = _transform.Find("RoofHitCheck").gameObject.transform.GetChild(i).transform.GetComponent<RoofCheck>();


    }

    void ReadInput()
    {
        if (_ground && !wallSlide && !wallJump && canJump && inputBuff == InputBuffer.jump && !isSliding && !isDashing)
            status = State.jumping;
        if (_notJump && !_ground && !isRolling && !wallSlide && !wallJump && !isDashing || rb.velocity.y < 0 && !_ground && !isRolling && !wallSlide && !wallJump && !isDashing)
            status = State.falling;

        if (inputBuff == InputBuffer.dash && !isDashing && !wallJump && !isSliding && !wallSlide && finalStamina >= 40 && !isRolling)
            if (horizontal != 0 || vertical != 0)
                status = State.dashing;
        if (!_ground && _wall && !wallJump && !isRolling)
            status = State.wallslide;

        if (inputBuff == InputBuffer.roll && _ground && !isRolling && finalStamina >= 20 && !_slope && !isDashing)
            status = State.rolling;

        if (inputBuff == InputBuffer.roll && _ground && !isRolling && finalStamina >= 20 && _slope)
            status = State.sliding;


        if (isFacingRight && horizontal < 0f && !isRolling && !isSliding || !isFacingRight && horizontal > 0f && !isRolling && !isSliding)
            Flip();

        animator.SetBool("Rolling", isRolling);
        animator.SetBool("Grounded", _ground);
        animator.SetFloat("AirSpeedY", rb.velocity.y);
        animator.SetBool("WallSlide", _wall);


        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        _jump = Input.GetButtonDown("Jump");
        _notJump = Input.GetButtonUp("Jump");
        _s = Input.GetKeyDown(KeyCode.S);
        _shift = Input.GetKeyDown(KeyCode.LeftShift);

        _transform = transform;
        _slope = isSlope();
        _ground = groundCheck.isTouching || isSlope();
        _wall = isWalled();
        _roll = isRolled();

        em.xVel = rb.velocity.normalized.x;
        em.yVel = rb.velocity.normalized.y;


        KayoteTime();
        InputBuff();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("player"), LayerMask.NameToLayer("enemy"), isRolling || isDashing);

    }
    public void parryDist(Vector2 dist)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dist.x * -facing, dist.x));
    }
    void CheckPosition()
    {
        if (_wall && !_ground && !wallJump && !wallSlide)
            status = State.wallslide;
        else if (!_wall && !wallJump && !_ground)
            status = State.falling;
        else if (_ground && horizontal == 0)
            status = State.idle;
        else if (_ground && horizontal != 0 && !_slope)
            status = State.walking;
        else if (_ground && horizontal != 0 && _slope)
            status = State.sloping;
    }
    protected override void ResetBools()
    {
        if(!_ground&&status==State.jumping)
        kayoteTimeTemp = 0;
        canMax = true;
        if (!_slope || _slope && isSliding)
            canJump = true;
        wallSlide = false;
        wallJump = false;
        canRoof = true;
        isDashing = false;
        isSliding = false;
        StopRolling();

     
        rb.gravityScale = gravity;
        rb.drag = drag;
    }

    void RemoveInputBuff()
    {
        StopCoroutine(InputBuffTimer());
        if (inputs.Count != 0)
            inputs.Remove(inputs[0]);

    }
    IEnumerator InputBuffTimer()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        RemoveInputBuff();
    }
    void InputBuff()
    {
        if (_jump)
        {
            inputs.Add(InputBuffer.jump);
            StartCoroutine(InputBuffTimer());
        }
        if (_s&&_ground|| _s && _slope)
        {
            inputs.Add(InputBuffer.roll);
            StartCoroutine(InputBuffTimer());
        }
        if (_shift)
        {
            inputs.Add(InputBuffer.dash);
            StartCoroutine(InputBuffTimer());
        }
        if (inputs.Count != 0)
            inputBuff = inputs[0];
        else
            inputBuff = InputBuffer.none;

    }
  
    void KayoteTime()
    {
        if (_ground)
            kayoteTimeTemp = kayoteTime;

        if (!kayote && !_ground && kayoteTimeTemp > 0 && !wallSlide)
            kayote = true;
        else if (kayoteTimeTemp > 0 && !wallSlide && !_ground)
        {
            kayote = true;
            kayoteTimeTemp = kayoteTimeTemp - 1 * Time.deltaTime;
        }
        else
            kayote = false;

    }
   
    void Roll()
    {
        Stamina(-1500f);
        RemoveInputBuff();
        rollTemp = rollTime;
        rb.drag = 0.1f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        isRolling = true;

        bc.enabled = false;
        bc2.enabled = true;

        animator.SetTrigger("Roll");

        rb.AddForce(Vector2.right * (facing * rollForce), ForceMode2D.Impulse);
    }
    void StopRolling()
    {
        isRolling = false;

        bc.enabled = true;
        bc2.enabled = false;
        rb.drag = drag;
    }
    void Sliding()
    {
        RemoveInputBuff();
        if (facing != Math.Round(rb.velocity.normalized.x))
            Flip();
        rb.bodyType = RigidbodyType2D.Dynamic;
        isSliding = true;
        IncGravity();
    }
    void Run(float speedLerpThingy = 1)
    {
        float force = Mathf.Lerp(minSpeed, speed, speedLerpThingy);
        ResetBools();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.right * horizontal * force;
        PlayRun();
    }
    void PlayRun()
    {
        if (Math.Abs(horizontal) > 0.1)
            animator.SetInteger("AnimState", 1);

        else if (horizontal == 0 && _ground)
            status = State.idle;
    }
    void RunAir(float speedLerpThingy = 1)
    {
        float finalSpeed;
        float force = Mathf.Lerp(minSpeed, airSpeed, speedLerpThingy);
        rb.bodyType = RigidbodyType2D.Dynamic;
        float targetSpeed = horizontal * force;
        float speedDiff = Math.Abs(targetSpeed - rb.velocity.x);
        finalSpeed = Mathf.Clamp(speedDiff * targetSpeed, -force, force);
        if (horizontal == -1 && rb.velocity.x >= finalSpeed || horizontal == 1 && rb.velocity.x <= finalSpeed)
        {
            if (isRolling && isFacingRight && rb.velocity.x > 0 || isRolling && !isFacingRight && rb.velocity.x < 0 || rb.velocity.x == 0 && isFacingRight && horizontal == 1 || rb.velocity.x == 0 && !isFacingRight && horizontal == -1 || !isRolling)   
                rb.AddForce(Vector2.right * finalSpeed * acceleration);
            
        }


    }
    void RunSlope(float speedLerpThingy = 1)
    {
        float force = Mathf.Lerp(minSpeed, slopeSpeed, speedLerpThingy);
        rb.drag = drag;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector2(slopeSpeed * slopeNormalPerp.x * -horizontal, force * slopeNormalPerp.y * -horizontal));
        PlayRun();
    }
    IEnumerator SpawnTrail()
    {
        canSpawnTrail = false;
        Instantiate(trail, _transform.position, Quaternion.identity);
        yield return new WaitForSeconds(trailSpawnRate);
        canSpawnTrail = true;
    }
    void WallSlide()
    {
        rb.gravityScale = gravity;
        isDashing = false;
        animator.SetTrigger("WallSlide");
        rb.velocity = new Vector2(rb.velocity.x, Math.Clamp(rb.velocity.y, -wallG, float.MaxValue));
        wallSlide = true;
    }
    IEnumerator WallJump()
    {
        jumpDir = -facing;
        RemoveInputBuff();
        rb.gravityScale = gravity;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(rb.velocity.x + (jumpDir * wallJumpPower.x), wallJumpPower.y), ForceMode2D.Impulse);
        if (facing != jumpDir)
            Flip();
        wallJump = true;
        yield return new WaitForSeconds(wallJumpTimer);
        wallJump = false;

    }
    public void Stamina(float amount)
    {
        if (amount > 0 && timerTemp <= 0 && waitStamina)
            timerTemp = staminaWaitTime;

        else if (amount < 0)
            waitStamina = true;

        else if (amount > 0 && timerTemp > 0)
            timerTemp -= Time.deltaTime;

        if (timerTemp <= 0 || amount < 0)
        {

            if (amount > 0)
                waitStamina = false;

            finalStamina = Mathf.Clamp(finalStamina + amount * Time.deltaTime, 0, stamina);
        }
    }
    void RoofCheck(int start)
    {
        if (roofChecks[start].roof && !roofChecks[start + 1].roof && !roofChecks[start + 2].roof && !roofChecks[start + 3].roof)
        {
            canRoof = false;
            _transform.Translate(.6f, 0, 0);

        }
        else if (!roofChecks[start].roof && !roofChecks[start + 1].roof && !roofChecks[start + 2].roof && roofChecks[start + 3].roof)
        {
            canRoof = false;
            _transform.Translate(-.6f, 0, 0);
        }

    }
    void Dash()
    {
        Stamina(-1980f);
        RemoveInputBuff();
        isRolling = false;
        dashTimeTemp = dashTimer;
        isDashing = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        Vector2 direction = new Vector2(horizontal, vertical);
        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);

    }
    bool isRolled()
    {

        return Physics2D.OverlapCircle(rollCheck.position, .1f, canWalk);
    }

    bool isSlope()
    {

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.gameObject.transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Slope"));

        if (hit)
        {
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.green);
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;


            return hit;

        }
        return wallSlope();

    }
    bool wallSlope()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(wallCheck.gameObject.transform.position, Vector2.left, .5f, LayerMask.GetMask("Slope"));
        if (hit)
            return true;
        hit = Physics2D.Raycast(wallCheck2.gameObject.transform.position, Vector2.right, .5f, LayerMask.GetMask("Slope"));
        if (hit)
            return true;
        return false;
    }
    public void Respawn()
    {
        ResetBools();
        status = State.idle;
        inputs.Clear();
    }

    private void OnDrawGizmos()
    {

        if (bc && groundCheck)
            Gizmos.DrawWireCube(groundCheck.gameObject.transform.position, new Vector2(bc.size.x, 0.1f));
    }
}
