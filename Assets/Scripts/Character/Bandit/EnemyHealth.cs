using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : CharacterHealth
{
    [SerializeField] float recoveryHp;
    public override void Death()
    {
        cm.Death();
    }


   public override void Respawn()
    {
        hp=recoveryHp;
    }
}
