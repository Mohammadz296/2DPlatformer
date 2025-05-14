using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    CapsuleCollider2D cc;
     void Start()
    {
        cc = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();

    }
   
    void OnDrawGizmos()
    {
        if (cc)
            Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - cc.size.y / 2), new Vector3(transform.position.x, transform.position.y + cc.size.y / 2));
    }
}
