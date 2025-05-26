using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : CharacterAttack
{
    Movementscript controller;
    PlayerAnim anim;
    public bool canParry { get; private set; }
    public bool click1 { get; private set; }
    bool staminaIsZero;
    bool isShield;
    [SerializeField] float parryOffTime;
    private void Awake()
    {
        canParry = true;
    }
    private void Start()
    { 
        anim = GetComponent<PlayerAnim>();
        cm = GetComponent<CharacterManager>();
        controller = GetComponent<Movementscript>();

    }
    void Update()
    {

        if (!staminaIsZero)
            click1 = Input.GetMouseButton(1);
        else click1 = false;
        cm.isShielding = click1;

        if (!staminaIsZero && controller.finalStamina == 0)
            staminaIsZero = true;
        else if (staminaIsZero && controller.finalStamina >= 30)
            staminaIsZero = false;
        if (!cm.isDead && !reload && Input.GetButtonDown("Fire1") && !controller.isRolling)
            playAttack();
        if (click1)
            controller.Stamina(-75f);
        if (cm.successParry)
            controller.finalStamina += 50f;
    }
    IEnumerator Parrytime()
    {
        canParry = false;

        yield return new WaitForSeconds(parryOffTime);
        canParry = true;
    }

    protected override void playAttack()
    {
        if (!click1)
        {
            StartCoroutine(Reload());
            anim.Attack();
        }
        else if(canParry)
        {
           
            anim.Block();
            StartCoroutine(Parrytime());
        }
    }
}
