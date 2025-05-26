using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public  abstract class CharacterAttack : MonoBehaviour
{
    protected bool reload;

    protected CharacterManager cm;
    [SerializeField] float parryTime;
    [SerializeField] protected float atk;
    [SerializeField] protected float attackSpeed;
    [field:SerializeField] public float range { get; private set; }

    public void Attack(List<CharacterManager> enemies)
    {

        foreach (CharacterManager enemy in enemies)
            if (!enemy.isDead && enemy.isDamageable)
            {
                if (!enemy.isParrying)
                    enemy.gameObject.GetComponent<CharacterHealth>().TakeDamage(atk);
                else
                    StartCoroutine(Parry());
            }


    }
    protected IEnumerator Reload()
    {
        reload = true;
        yield return new WaitForSeconds(attackSpeed);
        reload = false;
    }
    protected IEnumerator Parry()
    {
        cm.successParry = true;
        cm.GotParried();
        yield return 1;
        cm.successParry = false;
    }
    public IEnumerator ParryTime()
    {
        cm.isParrying = true;
        yield return new WaitForSeconds(parryTime);
        cm.isParrying = false;
    }
    protected abstract void playAttack();

}
