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
    bool isJumping;
    Vector3 distanceToHome;
    Vector3 home;
    [SerializeField] float waunderDistance;
    bool canWander = true;
    [SerializeField] float wanderTime;
   
    
    bool f;

    bool _wall;
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

    void Start()
    {
        wallCheck = transform.Find("WallCheck").GetComponent<EnvironmentCheck>();
        wallCheck2 = transform.Find("WallCheck2").GetComponent<EnvironmentCheck>();
        groundCheck = transform.Find("GroundCheck").GetComponent<EnvironmentCheck>();
        animator = transform.GetComponentInChildren<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        enemy = gameObject.GetComponent<enemy>();
        finalSpeed = speed;
        skin = transform.GetComponentInChildren<CharacterEvent>();
        facing = skin.facing;
        wallCheck.canWalk = canWalk;
        wallCheck2.canWalk = canWalk;  
        groundCheck.canWalk = canWalk;
        gravity=rb.gravityScale;
    }

    private void Update()
    {

        if (!enemy.isDead)
        {
            _ground = groundCheck.isTouching;
            _wall = isWalled();

            f = Input.GetKeyDown(KeyCode.F);
            animator.SetBool("Grounded", _ground);
           // animator.SetFloat("AirSpeedY", rb.velocity.y);
             
            if (distance.x > 0.1)
                horizontal = 1;
            else if (distance.x < -0.1)
                horizontal = -1;
            else
                horizontal = 0;
            if (enemy.isImmobile)
            {
                status = state.Immobilized;
            }
            else if (f && _ground && canJump)
            {
                status = state.Jumping;
            }
            else if (rb.velocity.y < 0 && !_ground)
            {
                status = state.Falling;
            }
            else if (isFighting)
            {
                status = state.Fighting;
            }
          
        }
        else
            status=state.death;
    }

    void FixedUpdate()
    {
        if (!enemy.isDead)
        {
            switch (status)
            {
                case state.Moving:
                    ResetBools();
                    MoveTowardsPlayer();
                    if (!isFollowing)
                        CheckPosition();
                    break;
                case state.idle:
                    IdleBehaviour();
                    CheckPosition();
                    break;
                case state.Fighting:
                    playRun();
                    idleStatus = IdleStatus.NewHome;
                    rb.velocity = Vector3.zero;
                    enemy.startAttack();
                    if (!isFighting)
                        CheckPosition();
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
                case state.Falling:

                    if (!_ground)
                    {
                        Falling();
                        MoveTowardsPlayer();
                    }
                    else
                        CheckPosition();
                    break; 
                    case state.death:
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
        rb.bodyType=RigidbodyType2D.Dynamic;
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
     if(isFollowing &&_ground && !isFighting)
            status=state.Moving;
     else if(!isFollowing && _ground&&!isFighting)
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
                if (canWander)
                {
                    Wander();
                    playRun();
                }
                else if (rb.velocity.x==0 && _ground)
                    Idle();

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
        rb.bodyType = RigidbodyType2D.Dynamic;
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


    }
    protected override void ResetBools()
    {
        canJump = true;
        canMax = true;
        wallSlide = false;
      rb.gravityScale = gravity;
    }
}

