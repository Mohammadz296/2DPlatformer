using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public event Action DeathEvent;
    public event Action RespawnEvent;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }
   
    public void Death()
    { 
        DeathEvent?.Invoke();
    }
    public void Respawn()
    {
        RespawnEvent?.Invoke();
    }
}
