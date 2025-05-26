using System.Collections;
using UnityEngine;

public abstract class CharacterManager : MonoBehaviour
{
    [SerializeField] protected int lives;
 public bool isDead;
    [HideInInspector] public bool isDamageable;
    [HideInInspector] public bool isImobile;
    [HideInInspector] public bool isParrying;
    [HideInInspector] public bool successParry;
    [HideInInspector] public bool isShielding;
    protected WaitForSeconds immobileWait;
    protected WaitForSeconds invulnerableWait;
    [field: SerializeField] public int immobleHitAmount { get; private set; }
    [SerializeField] float immobileTime;
    [SerializeField] protected Vector2 parryLaunchDist;
    [SerializeField] protected float invincibleTime;
    [HideInInspector]public int immobleHitAmountTemp;


    protected virtual void Awake()
    {
        immobileWait= new WaitForSeconds(immobileTime);
        invulnerableWait = new WaitForSeconds(invincibleTime);
        isDamageable = true;
    }
    protected void Reset()
    {
        isDead = false;
        isImobile = false;
    }
    public abstract void GotParried();
   
    public abstract void Death();
    public abstract void Respawn();
    public IEnumerator Invulnerable()
    {
        isDamageable = false;
        yield return invulnerableWait;
        isDamageable = true;
    }
    public void StopImmobilized()
    {
        immobleHitAmountTemp = immobleHitAmount;
        StopCoroutine(Immobilized());
        isImobile = false;
    }
    public IEnumerator Immobilized()
    {

        isImobile = true;
        yield return immobileWait;
        isImobile = false;
    }

}
