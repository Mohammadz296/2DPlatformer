using UnityEngine;

public class PlayerManager : CharacterManager
{
    Movementscript controller;
    Transform spawner;
     void Start()
    {
       
        controller = GetComponent<Movementscript>();
        spawner = GameObject.FindGameObjectWithTag("Spawn").transform;
        GameManager.Instance.RespawnEvent += Respawn;
        GameManager.Instance.DeathEvent += Death;   
    }
    public override void GotParried()
    {
        controller.parryDist(parryLaunchDist);
        StartCoroutine(Immobilized());
    }
    public override void Death()
    {
       
        isDead = true;
        
    }
    public override void Respawn()
    {
        
            Reset();
            transform.position = spawner.position;
            isDead = false;

        
    }
}
