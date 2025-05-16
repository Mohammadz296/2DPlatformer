using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public abstract class Character : MonoBehaviour
{
    protected bool clickReload = false;
    public float hp;
    protected bool reload = false;
    public float range = 3;
    [SerializeField] protected float immobileTime;
    public bool isImmobile;
    [SerializeField] protected float attackSpeed = 3;
    [SerializeField] protected Vector2 parryLaunchDist;
    protected float timer = 0;
    protected float minHp;
    protected bool isDamageable = true;
    public float defense;
    [HideInInspector] public bool isSheild;
    public float atk;
    [SerializeField] protected float parryTime;
    public bool isParrying;
    [SerializeField] protected float invincibleTime = .3f;
    [HideInInspector] public bool isDead = false;
    [SerializeField] protected int immobleHitAmount;
    protected bool successParry;
    protected int immobleHitAmountTemp;
    protected void Awake()
    {
        immobleHitAmountTemp = immobleHitAmount;
    }
    protected void takeDamage(float dmg)
    {
        if (!isDead && this.isDamageable)
        {

           
            if (isImmobile)
                immobleHitAmountTemp = Math.Clamp(immobleHitAmountTemp - 1, 0, immobleHitAmount);
            if (immobleHitAmountTemp == 0 && isImmobile)
                StopImmobilized();

            if (dmg != 0)
                playHurt();
            if (isSheild)
            {
                minHp = 1;
                dmg = (dmg / 100) * 25;
            }
            else
                minHp = 0;

            if (defense > 0f)
                defense = Mathf.Clamp(defense - 30 * (dmg / 100), 0f, defense);

            if (dmg > 0)
                dmg = Mathf.Clamp(dmg - defense, 0f, dmg);

            if (hp > 0)
                hp = Mathf.Clamp(hp - dmg, minHp, hp);
            if (hp == 0f&&!isDead)
                Death();


            UIBars(hp);
            StartCoroutine(Invulnerable());
        }

    }
    protected IEnumerator Invulnerable()
    {

        isDamageable = false;
        StartInvulnerable();
        yield return new WaitForSeconds(invincibleTime);
        isDamageable = true;
        EndInvulnerable();
    }
    protected virtual void StartInvulnerable()
    {
        
    }
    protected virtual void UIBars(float health)
    {
        
    }

    protected virtual void EndInvulnerable()
    {
        
    }
   

    public IEnumerator Attack(List<Character> enemies)
    {

        foreach (Character enemy in enemies)
            if (!enemy.isDead && isDamageable && !reload)
            {
                if (!enemy.isParrying)
                    enemy.takeDamage(atk);
                else
                {
                    enemy.successParry = true;
                    GotParried();
                    yield return 1;
                    enemy.successParry = false;
                }

            }


        reload = true;
        yield return new WaitForSeconds(attackSpeed);
        reload = false;

    }
    protected abstract void playHurt();

    protected abstract void GotParried();
   
    protected void StopImmobilized()
    {
        immobleHitAmountTemp = immobleHitAmount;
        StopCoroutine(Immobilized());
        isImmobile = false;
    }
    protected IEnumerator Immobilized()
    {

        isImmobile = true;
        yield return new WaitForSeconds(immobileTime);
        isImmobile = false;
    }

    protected abstract void playAttack();

    public  void Death()
    {
  
            isDead = true;
            StartCoroutine(playDeath());
        
    }

    public IEnumerator ParryTime()
    {
        isParrying = true;
        yield return new WaitForSeconds(parryTime);
        isParrying = false;
    }
    protected abstract IEnumerator playDeath();
    


    
    protected IEnumerator pressAttack()
    {

        playAttack();
        clickReload = true;
        yield return new WaitForSeconds(attackSpeed);
        clickReload = false;
    }



}

