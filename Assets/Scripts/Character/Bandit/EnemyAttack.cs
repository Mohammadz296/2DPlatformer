using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : CharacterAttack
{
    ICharacterAnim anim;
     void Start()
    {
        anim = GetComponent<ICharacterAnim>();
        cm = GetComponent<CharacterManager>();
    }
   
    public void ClickAttack()
    {
        if(!reload)
            playAttack();
    }
    protected override void playAttack()
    {


        StartCoroutine(Reload());
        anim.Attack();
    }
}
