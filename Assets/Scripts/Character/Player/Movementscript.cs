using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movementscript : CharacterMovement
{
    [SerializeField] float speed;
    [SerializeField] float airSpeed;
    [SerializeField] float wallG;
    [SerializeField] float rollTime;
    [SerializeField] float acceleration;
    [SerializeField] float jumpBufferTime;
    [SerializeField] float wallJumpTimer;
    [SerializeField] float staminaWaitTime;
    [SerializeField] float minSpeed;
    [SerializeField] float dashTimer;
    [SerializeField] float slopeSpeed;
    [SerializeField] float trailSpawnRate;
    [SerializeField] float trailAmount;
    [SerializeField] GameObject trail;
    [SerializeField] Vector2 wallJumpPower;
    [SerializeField] Vector2 dashForce;
    [SerializeField] Vector2 surrUDLR;
    [HideInInspector] public float finalStamina;
    public bool isRolling { get; private set; }
    public bool deflected { get; private set; }



    PlayerAttack pa;
    CapsuleCollider2D bc;
    CircleCollider2D bc2;
    EnvironmentManager em;

    SliderScript staminaBar;
    RoofCheck[] roofChecks = new RoofCheck[16];
    RaycastHit2D hit;
    EnvironmentCheck rollCheck;
    PlayerAnim animP;
    Vector2 slopeNormalPerp;
    List<InputBuffer> inputs = new();
    WaitForSeconds _trailSpawnRate;
    WaitForSeconds _jumpbufferTime;
    WaitForSeconds _wallJumpTimer;
    State status;
    InputBuffer inputBuff;

    bool _jump;
    bool _shift;
    bool _slope;
    bool _wall;
    bool _s;
    bool _roll;
    bool _notJump;
    bool kayote;
    bool wallJump;
    bool isDashing;
    bool isSliding;
    bool isDead;
    bool waitStamina = true;
    bool canRoof = true;


    [field: SerializeField] public float stamina { get; private set; }
    [SerializeField] float rollForce;

    float rollTemp;
    float jumpDir;

    int vertical;
    float dashTimeTemp;
    float drag;
    float timerTemp;
    float kayoteTimeTemp;



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
        death

    }

    enum InputBuffer
    {
        none,
        jump,
        roll,
        dash,
    }
    private void Awake()
    {
        CacheValues();
    }
    void Start()
    {
        SetStart();
        StartCoroutine(SpawnTrail());
    }
    void Update()
    {
        if (!isDead)
        {

            ReadInput();
            setInput();
            InputBuff();
        }
    }
    private void FixedUpdate()
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
                    SurroundCheck();

                RunAir();
                if (rb.velocity.y > 0 && rb.velocity.y <= maxMaxThreshold && canMax)
                    MaxHeight();

                if (_ground && rb.velocity.y == 0)
                    CheckPosition();
                break;
            case State.walking:
                if (!animP.getBlock() && pa.canParry)
                    Run();
                else if (pa.canParry)
                    Run(0f);
                else
                    rb.velocity = Vector2.zero;
                if (!pa.click1)
                    Stamina(30f);
                CheckPosition();
                break;
            case State.sloping:
                RunSlope();
                Stamina(10f);
                CheckPosition();
                break;
            case State.wallslide:

                if (inputBuff == InputBuffer.jump && _wall && !wallJump)
                    StartCoroutine(WallJump());

                else if (!wallJump && _wall)
                    WallSlide();
                else if (!_wall)
                    CheckPosition();
                if (wallJump)
                    RunAir(0);

                else
                    RunAir();


                break;
            case State.idle:
                Idle();
                if (!pa.click1)
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
                KayoteTime();
                if (canRoof)
                    SurroundCheck();
                if (inputBuff == InputBuffer.jump && kayote && canJump)
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
            case State.death:
                break;

        }
    }
    void SetStart()
    {
        anim = GetComponent<ICharacterAnim>();
        animP = (PlayerAnim)anim;
        groundCheck = _transform.Find("groundCheck").GetComponent<EnvironmentCheck>();
        rollCheck = _transform.Find("rollCheck").GetComponent<EnvironmentCheck>();
        wallCheck = _transform.Find("wallCheck").GetComponent<EnvironmentCheck>(); ;
        wallCheck2 = _transform.Find("wallCheck2").GetComponent<EnvironmentCheck>(); ;
        em = _transform.GetComponentInChildren<EnvironmentManager>();
        skin = _transform.Find("character").GetComponent<CharacterEvent>();
        bc = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        pa = GetComponent<PlayerAttack>();
        bc2 = GetComponent<CircleCollider2D>();
        staminaBar = GameObject.Find("StaminaBar").GetComponent<SliderScript>();

        staminaBar.SetMaxValue(stamina);
        facing = skin.facing;
        groundCheck.canWalk = canWalk;
        wallCheck.canWalk = canWalk;
        wallCheck2.canWalk = canWalk;
        rollCheck.canWalk = canWalk;
        drag = rb.drag;
        gravity = rb.gravityScale;

        for (int i = 0; i < roofChecks.Length; i++)
            roofChecks[i] = _transform.Find("RoofHitCheck").gameObject.transform.GetChild(i).transform.GetComponent<RoofCheck>();
        GameManager.Instance.RespawnEvent += Respawn;
        GameManager.Instance.DeathEvent += Death;


    }
    void CacheValues()
    {
        _transform = transform;
        _jumpbufferTime = new WaitForSeconds(jumpBufferTime);
        _trailSpawnRate = new WaitForSeconds(trailSpawnRate);
        _wallJumpTimer = new WaitForSeconds(wallJumpTimer);


        rollTemp = rollTime;
        finalStamina = stamina;
        kayoteTimeTemp = kayoteTime;
        timerTemp = staminaWaitTime;
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








    }
    void setInput()
    {

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        _jump = Input.GetButtonDown("Jump");
        _notJump = Input.GetButtonUp("Jump");
        _s = Input.GetKeyDown(KeyCode.S);
        _shift = Input.GetKeyDown(KeyCode.LeftShift);

        _transform = transform;
        _slope = isSlope();
        _ground = groundCheck.isTouching || isSlope();
        _wall = isWalled();
        _roll = rollCheck.isTouching;

        em.xVel = rb.velocity.normalized.x;
        em.yVel = rb.velocity.normalized.y;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("player"), LayerMask.NameToLayer("enemy"), isRolling || isDashing);
    }
    public void parryDist(Vector2 dist)
    {
        rb.velocity = Vector2.zero;
        setTempVector(dist.x * -facing, dist.y);
        rb.AddForce(_tempVector);
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
        kayoteTimeTemp = kayoteTime;
        kayote = false;
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
        yield return _jumpbufferTime;
        RemoveInputBuff();
    }
    void InputBuff()
    {
        if (_jump)
        {
            inputs.Add(InputBuffer.jump);
            StartCoroutine(InputBuffTimer());
        }
        if (_s && _ground || _s && _slope)
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

        if (!kayote && kayoteTimeTemp > 0 && !wallSlide)
            kayote = true;
        else if (kayoteTimeTemp > 0)
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

        animP.Roll();

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
        if (facing != Mathf.Round(rb.velocity.normalized.x))
            Flip();
        rb.bodyType = RigidbodyType2D.Dynamic;
        isSliding = true;
        IncGravity();
    }
    void SurroundCheck()
    {
        if (rb.velocity.y > 0)
            RoofCheck(0, surrUDLR.x, -surrUDLR.y, -surrUDLR.x, -surrUDLR.y);
        else if (rb.velocity.y < 0 && rb.gravityScale == gravityMax)
            RoofCheck(4, surrUDLR.x, -surrUDLR.y, -surrUDLR.x, -surrUDLR.y);

        if (rb.velocity.x > 0)
            RoofCheck(8, -surrUDLR.y, surrUDLR.x, -surrUDLR.y, -surrUDLR.x);
        else if (rb.velocity.x < 0)
            RoofCheck(12, surrUDLR.y, surrUDLR.x, surrUDLR.y, -surrUDLR.x);
    }
    void Run(float speedLerpThingy = 1)
    {
        float force = Mathf.Lerp(minSpeed, speed, speedLerpThingy);
        ResetBools();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.right * horizontal * force;
        PlayRun();
    }

    void RunAir(float speedLerpThingy = 1)
    {
        float finalSpeed;
        float force = Mathf.Lerp(minSpeed, airSpeed, speedLerpThingy);
        rb.bodyType = RigidbodyType2D.Dynamic;
        float targetSpeed = horizontal * force;
        float speedDiff = Mathf.Abs(targetSpeed - rb.velocity.x);
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

        rb.AddForce(force * slopeNormalPerp * -horizontal);
        PlayRun();
    }
    void PlayRun()
    {
        if (Mathf.Abs(horizontal) > 0.1)
            anim.Move(horizontal);

        else if (horizontal == 0 && _ground)
            status = State.idle;
    }
    IEnumerator SpawnTrail()
    {
        for (int i = 0; i < trailAmount;)
        {

            Instantiate(trail, _transform.position, Quaternion.identity);
            yield return _trailSpawnRate;
            i++;

        }

    }
    void WallSlide()
    {
        kayote = false;
        kayoteTimeTemp = 0;
        rb.gravityScale = gravity;
        isDashing = false;
        animP.WallSlide();
        setTempVector(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallG, jumpPower));
        rb.velocity = _tempVector;
        wallSlide = true;

    }
    IEnumerator WallJump()
    {
        wallJump = true;
        jumpDir = -facing;
        RemoveInputBuff();
        rb.gravityScale = gravity;
        rb.velocity = Vector2.zero;
        setTempVector(rb.velocity.x + (jumpDir * wallJumpPower.x), rb.velocity.y + wallJumpPower.y);
        rb.AddForce(_tempVector, ForceMode2D.Impulse);
        if (facing != jumpDir)
            Flip();
        yield return _wallJumpTimer;
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
        staminaBar.SetValue(finalStamina);  
    }
    void RoofCheck(int start, float x1, float y1, float x2, float y2)
    {
        if (roofChecks[start].roof && !roofChecks[start + 1].roof && !roofChecks[start + 2].roof && !roofChecks[start + 3].roof)
        {
            canRoof = false;
            _transform.Translate(x1, y1, _transform.position.z);

        }
        else if (!roofChecks[start].roof && !roofChecks[start + 1].roof && !roofChecks[start + 2].roof && roofChecks[start + 3].roof)
        {
            canRoof = false;
            _transform.Translate(x2, y2, _transform.position.z);
        }

    }
    void Dash()
    {
        Stamina(-1980f);
        RemoveInputBuff();
        isRolling = false;
        kayoteTime = 0;
        kayote = false;
        dashTimeTemp = dashTimer;
        isDashing = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        setTempVector(horizontal, vertical);
        Vector2 direction = _tempVector;
        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);

    }


    bool isSlope()
    {

        hit = Physics2D.Raycast(groundCheck.gameObject.transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Slope"));

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

        hit = Physics2D.Raycast(wallCheck.gameObject.transform.position, Vector2.left, .5f, LayerMask.GetMask("Slope"));
        if (hit)
            return true;
        hit = Physics2D.Raycast(wallCheck2.gameObject.transform.position, Vector2.right, .5f, LayerMask.GetMask("Slope"));
        if (hit)
            return true;
        return false;
    }
    void Death()
    {
        isDead = true;
    }
    void Respawn()
    {
        isDead = false;
        ResetBools();
        status = State.idle;
        inputs.Clear();
    }
    protected override bool isWalled()
    {
        if (isFacingRight && !_ground)
            return wallCheck.isTouching;
        else if (!isFacingRight && !_ground)
            return wallCheck2.isTouching;
        else
            return false;
    }


}
