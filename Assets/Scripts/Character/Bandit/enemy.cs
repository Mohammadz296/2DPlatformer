using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemy : Character
{
    [SerializeField] int lives;
    [SerializeField] float recoverHp;
    Animator animator;
    BanditController controller;
    Collider2D box;
    Collider2D box2;
    SpriteRenderer skin;


    private void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        controller = gameObject.GetComponent<BanditController>();
        box = gameObject.GetComponent<Collider2D>();
        box2 = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        Physics2D.queriesHitTriggers = false;
        skin=gameObject.GetComponentInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        if (isImmobile)
            skin.color = Color.gray;
        else
            skin.color = Color.white;
        animator.SetBool("isDead", isDead);
    }
    public void startAttack()
    {
        if (!clickReload)
            StartCoroutine(pressAttack());

    }
    protected override void GotParried()
    {
        controller.parryDist(parryLaunchDist);
        StartCoroutine(Immobilized());
    }
    protected override void playHurt()
    {
        animator.SetTrigger("Hurt");

    }
    protected override void playAttack()
    {
        animator.SetTrigger("Attack");
    }
    protected override IEnumerator playDeath()
    {
        isDead = true;
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Attack");
        Physics2D.IgnoreCollision(box, box2, true);
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(2f);
        if (lives == 0)
            Destroy(gameObject);
        else
            StartCoroutine(Revive());
    }
    IEnumerator Revive()
    {
        lives--;
        animator.SetTrigger("Recover");
        yield return new WaitForSeconds(1);
        hp = recoverHp;
        isDead = false;
        Physics2D.IgnoreCollision(box, box2, false);

    }
}
