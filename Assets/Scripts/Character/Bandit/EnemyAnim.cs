using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnim : MonoBehaviour,ICharacterAnim
{
    Animator animator;
    CharacterManager cm;
    SpriteRenderer sprite;
    BanditController controller;

    void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        sprite= animator.gameObject.GetComponent<SpriteRenderer>();
        controller=GetComponent<BanditController>();
        cm=GetComponent<CharacterManager>();
        
    }
    void Update()
    {
        Immobile();
        animator.SetBool("Grounded", controller._ground);
    }
    public void Immobile()
    {
        if (cm.isImobile)
            sprite.color = Color.gray;
        else
            sprite.color = Color.white;
    }
    public void Move(int x)
    {
        animator.SetInteger("AnimState", x);
    }
    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }
    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
    public void Death()
    {
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Death");
    }
    public void Revive()
    {
        animator.SetTrigger("Recover");
    }
    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
}
