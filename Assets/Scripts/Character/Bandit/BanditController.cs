using Pathfinding.Util;
using System;
using System.Collections;
using UnityEngine;

public class BanditController : CharacterMovement
{
    [SerializeField] float speed;
    float finalSpeed;

    [SerializeField] float homeDistance;
    public Vector2 distance = Vector2.zero;
    Vector3 localScale;

  

    [HideInInspector] public bool isFighting = false;
    [HideInInspector] public bool isFollowing = false;
    enemy enemy;
    state status;
    IdleStatus idleStatus;

    Vector3 destination;

    float amount;
    CharacterEvent characterEvent;
    Vector3 distanceToHome;
    Vector3 home;
    [SerializeField] float waunderDistance;
    bool canWander = true;
    [SerializeField] float wanderTime;


    bool _wall;
    enum state
    {
        Moving,
        Fighting,
        idle,
        Immobilized,
        Falling,
        Jumping
    }
    enum IdleStatus
    {
        NewHome,
        Tohome,
        Waunder,
    }

    void Start()
    {
        wallCheck = transform.Find("WallCheck").GetComponent<EnvironmentCheck>();
        wallCheck2 = transform.Find("WallCheck2").GetComponent<EnvironmentCheck>();
        groundCheck = transform.Find("GroundCheck").GetComponent<EnvironmentCheck>();
        animator = transform.GetComponentInChildren<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        enemy = gameObject.GetComponent<enemy>();
        finalSpeed = speed;
        characterEvent = transform.GetComponentInChildren<CharacterEvent>();
        facing = characterEvent.facing;
        wallCheck.canWalk = canWalk;
        wallCheck2.canWalk = canWalk;   
    }

    private void Update()
    {

        if (!enemy.isDead)
        {
            _ground = groundCheck.isTouching;
            _wall = isWalled();

            animator.SetBool("Grounded", _ground);

            if (distance.x > 0.1)
                horizontal = 1;
            else if (distance.x < -0.1)
                horizontal = -1;
            else
                horizontal = 0;
            if (rb.velocity.y < 0)
                status =state.Falling;
            if (enemy.isImmobile)
                status = state.Immobilized;
            else if (isFighting && !enemy.isImmobile)
                status = state.Fighting;
            else if (isFollowing && !enemy.isImmobile)
                status = state.Moving;
            else
                status = state.idle;
           
        }
    }

    void FixedUpdate()
    {
        if (!enemy.isDead)
        {
            switch (status)
            {
                case state.Moving:

                    MoveTowardsPlayer();
                    break;
                case state.idle:
                    Idle();
                    IdleBehaviour();

                    break;
                case state.Fighting:
                    playRun();
                    idleStatus = IdleStatus.NewHome;
                    rb.velocity = Vector3.zero;
                    enemy.startAttack();

                    break;
                case state.Immobilized:
                    animator.SetInteger("AnimState", 0);
                    break;
                case state.Jumping:

                    if (canJump)
                        Jump();
                    MoveTowardsPlayer();
                    if (rb.velocity.y > 0 && rb.velocity.y <= maxMaxThreshold && canMax)
                        MaxHeight();

                    if (_ground && rb.velocity.y == 0)
                        CheckPosition();
                    break;
            }


            if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
                Flip();
        }
        else
            rb.velocity = Vector3.zero;

    }
    public void MoveTowardsPlayer()
    {
        playRun();
        finalSpeed = Mathf.Clamp(speed * Math.Abs(distance.x), speed, 6);
        rb.velocity = new Vector2(horizontal * finalSpeed, rb.velocity.y);

    }
    void NewHome()
    {

        home = transform.position;
        destination = Vector3.zero;
    }
    void CheckPosition()
    {
         if (_ground && horizontal == 0)
            status = state.idle;
        else if (_ground && horizontal != 0)
            status = state.Moving;
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
                if (canWander)
                    Wander();


                WaunderValues();
                if (_wall || Math.Abs(distanceToHome.x) >= homeDistance)
                idleStatus = IdleStatus.Tohome;
                
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
        rb.velocity = new Vector2(dist.x * -facing, dist.y);
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
        if (Math.Abs(distance.x) <= 0.1f)
        {
            amount = UnityEngine.Random.Range(-waunderDistance, waunderDistance);
            destination = new Vector3(transform.position.x + amount, transform.position.y, transform.position.z);

        }
        if(canWander)
        StartCoroutine(WaunderTime());


    }
    IEnumerator WaunderTime()
    {
        canWander = false;
        yield return new WaitForSeconds(wanderTime);
        canWander = true;
    }

  


    void playRun()
    {
        if (Math.Abs(rb.velocity.x) > 0.1 && !isFighting)
            animator.SetInteger("AnimState", 2);
        else if (isFighting)
            animator.SetInteger("AnimState", 1);
        else
            status = state.idle;
    }
    protected override void ResetBools()
    {
        canMax = true;
        wallSlide = false;
      rb.gravityScale = gravity;
    }
}

