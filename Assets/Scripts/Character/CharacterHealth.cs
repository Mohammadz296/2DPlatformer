using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(ICharacterAnim))]
public abstract class CharacterHealth : MonoBehaviour
{
    [field: SerializeField] public float hp { get; protected set; }
    [field: SerializeField] public float defense { get; protected set; }
    protected int minHp;


    protected ICharacterAnim anim;
    protected  CharacterManager cm;


    protected void Start()
    {
        anim = GetComponent<ICharacterAnim>();
        cm = GetComponent<CharacterManager>();
    }
    public virtual void TakeDamage(float dmg)
    {
        if (cm.isImobile)
            cm.immobleHitAmountTemp = Math.Clamp(cm.immobleHitAmountTemp - 1, 0, cm.immobleHitAmount);
        if (cm.immobleHitAmountTemp == 0 && cm.isImobile)
            cm.StopImmobilized();
        if (cm.isShielding)
            minHp = 1;
        else
            minHp = 0;
        if (defense > 0f)
            defense = Mathf.Clamp(defense - 30 * (dmg / 100), 0f, defense);

        if (dmg > 0)
        {
            anim.Hurt();
            dmg = Mathf.Clamp(dmg - defense, 0f, dmg);
        }

        if (hp > 0)
            hp = Mathf.Clamp(hp - dmg, minHp, hp);
        if (hp == 0f)
            Death();
        StartCoroutine(cm.Invulnerable());
    }
  
    public abstract void Respawn();

    public abstract void Death();
  
}
