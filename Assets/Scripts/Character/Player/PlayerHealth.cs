using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : CharacterHealth
{
    SliderScript healthBar;
    SliderScript defenseBar;
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
       healthBar= GameObject.Find("HealthBar").GetComponent<SliderScript>();
       defenseBar= GameObject.Find("DefenseBar").GetComponent<SliderScript>();
        resetBars();
    }
    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        healthBar.SetValue(hp);
        defenseBar.SetValue(defense);   

    }
    void resetBars()
    {
        healthBar.SetMaxValue(hp);
        defenseBar.SetMaxValue(defense);    
    }


    public override void Death()
    {
        StartCoroutine(GameManager.Instance.Death());
    }
    public override void Respawn()
    {
        hp=ogHp;
        defense=ogDef;  
        resetBars();
    }

}
