using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{ 
    [field: SerializeField]public float hp { get; private set; }
    [field: SerializeField] public float defense { get; private set; }
    int minHp;
     float ogHp;
     float ogDefense;
    bool isDead;
    public event Action DeathEvent;


    private void Start()
    {
        ogHp = hp;
        ogDefense = defense;
        GameManager.Instance.RespawnEvent += ResetStats;
    }
    void TakeDamage(float dmg, bool isShield)
    {
        if (isShield)
            minHp = 1;
        else
            minHp = 0;
        if (defense > 0f)
            defense = Mathf.Clamp(defense - 30 * (dmg / 100), 0f, defense);

        if (dmg > 0)
            dmg = Mathf.Clamp(dmg - defense, 0f, dmg);

        if (hp > 0)
            hp = Mathf.Clamp(hp - dmg, minHp, hp);
        if (hp == 0f)
            GameManager.Instance.Death();

    }
    void ResetStats()
    {
   hp=ogHp;
        defense = ogDefense;
    }
    public void Death()
    {
        DeathEvent?.Invoke();

    }
}
