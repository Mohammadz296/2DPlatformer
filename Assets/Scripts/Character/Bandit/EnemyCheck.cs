using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheck : MonoBehaviour
{
     LayerMask layer;
    Collider2D[] hit;
    [SerializeField] Vector2 size = new Vector2(24, 13);
   [SerializeField]  Vector2 sphereCenter = new Vector2(3.5f, 2.5f);
    Animator animator;
    bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();   
        layer = LayerMask.GetMask("enemy");
        Physics2D.queriesHitTriggers = false;
    }

    // Update is called once per frame
    void Update()
    {
       hit= Physics2D.OverlapBoxAll(sphereCenter, size,0, layer);
        
       if (hit.Length == 0&&!isOpen)
        {
           isOpen = true;
            animator.SetTrigger("Open");
            
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(sphereCenter.x,sphereCenter.y,0), new Vector3(size.x, size.y, 0));
    }


}
