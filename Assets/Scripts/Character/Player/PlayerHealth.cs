using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : CharacterHealth
{
    float ogHp;
    float ogDef;
    private void Awake()
    {
        ogHp = hp;
        ogDef = defense;
    }
     new void Start()
    {
        base.Start();
        GameManager.Instance.RespawnEvent += Respawn;
        

    }
    
    public override void Death()
    {
        StartCoroutine(GameManager.Instance.Death());
    }
    public override void Respawn()
    {
        hp=ogHp;
        defense=ogDef;  
    }

}
