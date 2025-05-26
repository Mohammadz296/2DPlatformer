using System.Collections;
using UnityEngine;

public class PlayerStats : Character
{
    SliderScript healthBar;
    SliderScript defenseBar;
    SliderScript staminaBar;
    Animator animator;
    SpriteRenderer sprite;
    Movementscript controller;
    Spawner spawner;
    float ogHealth;
    float ogDefense;
    bool staminaIsZero;
    public bool canParry = true;
    [SerializeField] float parryOffTime;
    public bool click1;
    private void Start()
    {
        SetStart();
    }
    void Update()
    {
        if (!staminaIsZero)
            click1 = Input.GetMouseButton(1);
        else click1 = false;

        if (!staminaIsZero && controller.finalStamina == 0)
            staminaIsZero = true;
        else if (staminaIsZero && controller.finalStamina >= 30)
            staminaIsZero = false;

        isSheild = click1;
        animator.SetBool("IdleBlock", click1);
        if (staminaBar)
            staminaBar.SetValue(controller.finalStamina);

        if (!isDead && !clickReload && Input.GetButtonDown("Fire1") && !controller.isRolling)
            StartCoroutine(pressAttack());
        if (click1)
            controller.Stamina(-75f);
        if (successParry)
            controller.finalStamina += 50f;


    }

    void SetStart()
    {

        sprite = transform.Find("character").GetComponent<SpriteRenderer>();
        animator = transform.Find("character").GetComponent<Animator>();
        controller = GetComponent<Movementscript>();
        sprite = transform.Find("character").GetComponent<SpriteRenderer>();
        healthBar = GameObject.Find("Healthbar").GetComponent<SliderScript>();
        defenseBar = GameObject.Find("Defensebar").GetComponent<SliderScript>();
        staminaBar = GameObject.Find("Staminabar").GetComponent<SliderScript>();
        spawner = GameObject.Find("SpawnPoint").GetComponent<Spawner>();

        ogHealth = hp;
        ogDefense = defense;
        ResetStats();
        GameManager.Instance.RespawnEvent += Respawn;


    }

    
    protected override void UIBars(float health)
    {
        healthBar.SetValue(hp);
        defenseBar.SetValue(defense);
    }
    protected override void playHurt()
    {
        animator.SetTrigger("Hurt");
    }

    protected override void playAttack()
    {
        if (!click1)
        {

            int f = Random.Range(0, 3);
            switch (f)
            {
                case 0:
                    animator.SetTrigger("Attack1");
                    break;
                case 1:
                    animator.SetTrigger("Attack2");
                    break;
                case 2:
                    animator.SetTrigger("Attack3");
                    break;
                default:
                    animator.SetTrigger("attack1");
                    break;
            }
        }
        else
            if (canParry)
        {
            animator.SetTrigger("Block");
            StartCoroutine(Parrytime());
            
        }





    }
    IEnumerator Parrytime()
    {
        canParry = false;    
   
        yield return new WaitForSeconds(parryOffTime);      
        canParry = true;
    }
    protected override void GotParried()
    {
        controller.parryDist(parryLaunchDist);
        StartCoroutine(Immobilized());
    }
    public override void Death()
    {
        StartCoroutine(GameManager.Instance.Death());
        isDead = true;
        animator.SetBool("isDead", isDead);
        animator.SetTrigger("Death");
    }

   
    private void ResetStats()
    {
        controller.finalStamina = controller.stamina;
        hp = ogHealth;
        defense = ogDefense;
        defenseBar.SetMaxValue(defense);
        healthBar.SetMaxValue(hp);
        staminaBar.SetMaxValue(controller.stamina);

    }
    void Respawn()
    {
        ResetStats();
        transform.position = spawner.transform.position;
        isDead = false;
        animator.SetBool("isDead", isDead);

    }
}

