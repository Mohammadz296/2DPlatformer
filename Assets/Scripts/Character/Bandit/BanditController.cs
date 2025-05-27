using System;
using System.Collections;
using UnityEngine;

public class BanditController : CharacterMovement
{
    [SerializeField] float speed;
    [SerializeField] float homeDistance;
    [SerializeField] float jumpInterval;
    [SerializeField] float waunderDistance;
    [SerializeField] float wanderTime;
    [HideInInspector] public Vector2 distance { private get; set; }
    [HideInInspector] public bool isFighting;
    [HideInInspector] public bool isFollowing;
    EnemyAnim animE;
    float finalSpeed;
    float amount;

    bool canNotMove;
    bool isJumping;
    bool canWander = true;
    bool _wall;
    bool isDead;



    WaitForSeconds waunderWait;
    EnemyManager enemy;
    EnemyAttack enemyAtk;
    Collider2D bc;
    Collider2D bc2;
    state status;
    IdleStatus idleStatus;
    Vector3 localScale;
    Vector3 destination;
    Vector3 distanceToHome;
    Vector3 home;


    enum state
    {
        idle,
        Moving,
        Fighting,
        Immobilized,
        death,
        Falling,
        Jumping
    }
    enum IdleStatus
    {
        NewHome,
        Tohome,
        Waunder,
    }
    private void Awake()
    {
        CacheValues();
    }

    void Start()
    {
        SetStart();
    }

    private void Update()
    {

        _ground = groundCheck.isTouching;
        _wall = isWalled();




        if (!isDead)
        {
            if (distance.x > 0.1)
                horizontal = 1;
            else if (distance.x < -0.1)
                horizontal = -1;
            else
                horizontal = 0;
            if (enemy.isImobile)
                status = state.Immobilized;
            else if (isFighting)
                status = state.Fighting;
        }
    }

    void FixedUpdate()
    {

        switch (status)
        {
            case state.Moving:
                ResetBools();
                if (!isFollowing)
                    CheckPosition();
                else if (distance.y >= jumpInterval && canJump)
                    status = state.Jumping;
                MoveTowardsPlayer();
                break;
            case state.idle:
                CheckPosition();
                IdleBehaviour();
                break;
            case state.Fighting:

                Fight();
                if (!isFighting)
                    CheckPosition();
                break;
            case state.Immobilized:
                anim.Move(0);
                Imobilized();
                if (!enemy.isImobile)
                    status = state.idle;
                break;
            case state.Jumping:
                if (canJump && status != state.idle)
                    Jump();

                MoveTowardsPlayer();
                if (rb.velocity.y > 0 && rb.velocity.y <= maxMaxThreshold && canMax)
                    MaxHeight();

                if (_ground && rb.velocity.y == 0)
                    CheckPosition();
                break;
            case state.Falling:
                if (_ground)
                    CheckPosition();
                if (canJump && isFollowing)
                {
                    Jump();
                    status = state.Jumping;
                }
                Falling();
                MoveTowardsPlayer();

                break;
            case state.death:
                break;

        }


        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
            Flip();


    }
    void SetStart()
    {
        wallCheck = _transform.Find("WallCheck").GetComponent<EnvironmentCheck>();
        wallCheck2 = _transform.Find("WallCheck2").GetComponent<EnvironmentCheck>();
        groundCheck = _transform.Find("GroundCheck").GetComponent<EnvironmentCheck>();
        anim = GetComponent<ICharacterAnim>();
        animE = (EnemyAnim)anim;
        rb = gameObject.GetComponent<Rigidbody2D>();
        enemy = gameObject.GetComponent<EnemyManager>();
        enemyAtk = GetComponent<EnemyAttack>();
        bc = GetComponent<Collider2D>();
        bc2 = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        skin = _transform.GetComponentInChildren<CharacterEvent>();
        facing = skin.facing;
        wallCheck.canWalk = canWalk;
        wallCheck2.canWalk = canWalk;
        groundCheck.canWalk = canWalk;
        gravity = rb.gravityScale;
    }
    void Fight()
    {
        playRun();
        idleStatus = IdleStatus.NewHome;
        setTempVector(0, rb.velocity.y);
        rb.velocity = _tempVector;
        enemyAtk.ClickAttack();

    }
    void CacheValues()
    {
        waunderWait = new WaitForSeconds(wanderTime);
        _transform = transform;
        finalSpeed = speed;
        distance = Vector2.zero;
    }
    void MoveTowardsPlayer()
    {
        ResetBools();
        playRun();
        finalSpeed = Mathf.Clamp(speed * Math.Abs(distance.x), speed, 6);
        setTempVector(horizontal * finalSpeed, rb.velocity.y);
        rb.velocity = _tempVector;

    }
    void Imobilized()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (rb.velocity.y < 0)
            canNotMove = true;
        if (_ground && canNotMove)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
        }

    }
    public void Death()
    {
        Physics2D.IgnoreCollision(bc, bc2, true);
        rb.velocity = Vector2.zero;
        isDead = true;
        status = state.death;
        ResetBools();
        isFighting = false;
        isFollowing = false;
        distance = Vector3.zero;
        idleStatus = IdleStatus.NewHome;
    }
    public void Revive()
    {
        isDead = false;
        status = state.idle;
        Physics2D.IgnoreCollision(bc, bc2, false);
    }
    void NewHome()
    {

        home = _transform.position;
        destination = Vector3.zero;
    }
    void CheckPosition()
    {
        if (rb.velocity.y < 0 && !_ground)
            status = state.Falling;
        else if (isFollowing && _ground && !isFighting)
            status = state.Moving;
        else if (!isFollowing && _ground && !isFighting)
            status = state.idle;

    }


    void IdleBehaviour()
    {
        switch (idleStatus)
        {
            case IdleStatus.NewHome:
                NewHome();
                idleStatus = IdleStatus.Waunder;
                break;
            case IdleStatus.Waunder:
                if (isFollowing)
                    CheckPosition();
                else if (_wall || Math.Abs(distanceToHome.x) >= homeDistance)
                {
                    rb.velocity = Vector2.zero;
                    distance = Vector2.zero;
                    idleStatus = IdleStatus.Tohome;
                }
                if (canWander)
                {
                    Wander();
                    playRun();
                }
                else if (rb.velocity.x == 0 && _ground)
                    Idle();
                WaunderValues();


                break;
            case IdleStatus.Tohome:
                HomeDistance();
                destination = Vector3.zero;
                distance = distanceToHome;
                MoveTowardsPlayer();
                if (Math.Abs(distance.x) <= 0.1f)
                    idleStatus = IdleStatus.Waunder;
                break;

        }
    }
    public void parryDist(Vector2 dist)
    {
        rb.velocity = Vector3.zero;
        setTempVector(dist.x * -facing, dist.y);
        rb.velocity = _tempVector;
    }
    void HomeDistance()
    {
        distanceToHome = home - transform.position;
    }
    void WaunderValues()
    {
        distance = destination - transform.position;
        HomeDistance();
        MoveTowardsPlayer();

    }
    void Wander()
    {
        ResetBools();
        if (Math.Abs(distance.x) <= 0.1f)
        {
            amount = UnityEngine.Random.Range(-waunderDistance, waunderDistance);
            setTempVector(_transform.position.x + amount, _transform.position.y);
            destination = _tempVector;

        }
        if (canWander)
            StartCoroutine(WaunderTime());


    }
    IEnumerator WaunderTime()
    {
        canWander = false;
        yield return waunderWait;
        canWander = true;
    }




    void playRun()
    {
        if (Math.Abs(rb.velocity.x) > 0.1 && !isFighting)
            anim.Move(2);
        else if (isFighting)
            anim.Move(1);


    }
    protected override void ResetBools()
    {
        if (_ground)
        {
            canJump = true;
            canNotMove = false;
            canMax = true;
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        wallSlide = false;
        rb.gravityScale = gravity;
    }
}

