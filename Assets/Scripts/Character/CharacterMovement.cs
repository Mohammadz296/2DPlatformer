using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterMovement : MonoBehaviour
{
    [SerializeField] protected float jumpPower;
    [SerializeField] protected float maxHeightBoost;
    [SerializeField] protected float gravityMax;
    [SerializeField] protected float gravityForce;
    [SerializeField] protected float maxMaxThreshold;
    [SerializeField] protected LayerMask canWalk;

    protected bool _ground;
    protected bool wallSlide;
    protected bool isFacingRight=true;
    protected bool canJump = true;
    protected bool canMax = true;

    protected float horizontal;
    protected float facing;
    protected float gravity=0;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected CharacterEvent skin;
    protected EnvironmentCheck groundCheck;
    protected EnvironmentCheck wallCheck;
    protected EnvironmentCheck wallCheck2;
    protected Transform _transform;

    protected void Falling()
    {
        wallSlide = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        IncGravity();
    }
    protected void IncGravity()
    {
        float _targetSpeed = gravityMax - rb.gravityScale;
        rb.gravityScale = Mathf.Clamp(rb.gravityScale + _targetSpeed * Time.deltaTime * gravityForce, gravity, gravityMax);
    }
    protected void MaxHeight()
    {
        canMax = false;
        rb.gravityScale = rb.gravityScale / 2;
        rb.AddForce(maxHeightBoost * horizontal * Vector2.right);
    }
    protected void Jump()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        ResetBools();
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        canJump = false;
        animator.SetTrigger("Jump");
    }
    protected void Idle()
    {
        ResetBools();
        rb.gravityScale = gravity;
        rb.velocity = Vector2.zero;
        animator.SetInteger("AnimState", 0);
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        skin.flip();
        facing = skin.facing;
    }
    protected bool isWalled()
    {
        if ( isFacingRight)
            return wallCheck.isTouching;
        else if (!isFacingRight)
            return wallCheck2.isTouching;
        else
            return false;
    }
    protected abstract void ResetBools();

}
