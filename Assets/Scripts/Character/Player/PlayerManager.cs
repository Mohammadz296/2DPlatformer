using UnityEngine;

public class PlayerManager : CharacterManager
{
    Movementscript controller;
     void Start()
    {
        controller = GetComponent<Movementscript>();
        GameManager.Instance.RespawnEvent += Respawn;
        GameManager.Instance.DeathEvent += Death;
        Respawn();
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
            transform.position = GameObject.FindGameObjectWithTag("Spawn").transform.position;
            isDead = false;

        
    }
}
