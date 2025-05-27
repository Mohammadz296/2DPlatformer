using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    bool spawn;
    CapsuleCollider2D cc;
     void Start()
    {
        cc = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
        GameManager.Instance.SpawnEvent += ResetSpawn;
    }
   
    void OnDrawGizmos()
    {
        if (cc)
            Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - cc.size.y / 2), new Vector3(transform.position.x, transform.position.y + cc.size.y / 2));
    }
    void ResetSpawn()
    {
        if (spawn)
            gameObject.tag = "Spawn";
        else
            gameObject.tag = "Untagged";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spawn = true;
            GameManager.Instance.Spawn();
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            spawn = false;
    }

}
