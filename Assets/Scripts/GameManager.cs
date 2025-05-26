using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public event Action DeathEvent;
    public event Action RespawnEvent;
    public event Action pause;
    public event Action resume;
    [SerializeField] float respawnTime;
    WaitForSeconds respawnWait;
    [SerializeField] public GameStates state;
    public enum GameStates
    {
        Pause,
        Death,
        Respawn,
        Game,
    }
    private void Awake()
    {
        respawnWait = new WaitForSeconds(respawnTime);
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }
    
    public IEnumerator Death()
    {
        state = GameStates.Death;
        DeathEvent?.Invoke();
        yield return respawnWait;
        Respawn();
    }
    public void Respawn()
    {
        state = GameStates.Respawn;
        RespawnEvent?.Invoke();
    }
    private void Pause()
    {
        Time.timeScale = 0;
    }
    void UnPause()
    {
        Time.timeScale = 1;
    }

}
