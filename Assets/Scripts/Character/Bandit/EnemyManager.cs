using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : CharacterManager
{
    BanditController controller;
    WaitForSeconds DeathWait;
    WaitForSeconds RevWait;
    EnemyAnim anim;
    EnemyHealth hp;
     protected override void Awake()
    {
        base.Awake();
        DeathWait = new WaitForSeconds(1);
        RevWait = new WaitForSeconds(1);
    }
    private void Start()
    {
        hp=GetComponent<EnemyHealth>();
        controller = GetComponent<BanditController>();
        anim=GetComponent<EnemyAnim>();
    }
    public override void GotParried()
    {
        controller.parryDist(parryLaunchDist);
        StartCoroutine(Immobilized());
    }
    public override void Death()
    {
        controller.Death();
        isDead = true;
        anim.Death();
        if (lives != 0)
        {
            lives--;
            Respawn();
        }
        else
        StartCoroutine(Delete());
    }
    IEnumerator Delete()
    {
        yield return DeathWait;
        Destroy(gameObject);
    }
    public override void Respawn()
    {
        StartCoroutine(RespawnTimer());
    }
    IEnumerator RespawnTimer()
    {
        yield return DeathWait;
        anim.Revive();
        yield return RevWait ;
        isDead = false;
        controller.Revive();
        hp.Respawn();
    }

}
