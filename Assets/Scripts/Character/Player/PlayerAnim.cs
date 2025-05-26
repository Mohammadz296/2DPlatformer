using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnim : MonoBehaviour, ICharacterAnim
{
    PlayerManager cm;
    Animator animator;
    GameObject character;
    SpriteRenderer sprite;
    Movementscript controller;
    Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<Movementscript>();
        cm = GetComponent<PlayerManager>();
        character = transform.Find("character").gameObject;
        animator = character.GetComponent<Animator>();
        sprite = character.GetComponent<SpriteRenderer>();
        GameManager.Instance.DeathEvent += Death;
    }
    void Update()
    {
        animator.SetBool("Rolling", controller.isRolling);
        animator.SetBool("Grounded", controller._ground);
        animator.SetFloat("AirSpeedY", rb.velocity.y);
        animator.SetBool("WallSlide", controller.wallSlide);
        animator.SetBool("isDead", cm.isDead);
        animator.SetBool("IdleBlock", cm.isShielding);

    }
    public void Attack()
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

    public void Move(int x)
    {
        animator.SetInteger("AnimState", x);
    }
    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }
    public IEnumerator playImmobile(float time)
    {
        sprite.color = Color.gray;
        yield return new WaitForSeconds(time);
        EndEffects();
    }

    public void EndEffects()
    {
        sprite.color = Color.white;
    }
    public void Immobile()
    {
        sprite.color = Color.gray;
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
    public void Roll()
    {
        animator.SetTrigger("Roll");
    }
    public void WallSlide()
    {
        animator.SetTrigger("WallSlide");
    }
    public void Death()
    {
        animator.SetTrigger("Death");
    }
    public void Block()
    {
        animator.SetTrigger("Block");
    }
    public bool getBlock()
    {
        return animator.GetBool("IdleBlock");
    }
}
