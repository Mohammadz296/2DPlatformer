using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BanditFollowTrigger : MonoBehaviour
{
    BanditController bandit;
 
    public LayerMask mask;
    Transform target;
    bool isTarget;
    [SerializeField] float radius;
    Transform trailpos;
    Vector2 distance;
    RaycastHit2D hit;
    Collider2D[] colliders;
    List<Transform> trail = new List<Transform>();
    void Start()
    {
        bandit = gameObject.transform.parent.gameObject.GetComponent<BanditController>();

        Physics2D.queriesHitTriggers = false;
    }

    // Update is called once per frame

    void Update()
    {
        InvokeRepeating("Check", 0f,1f);
     

        Debug.DrawRay(transform.position, distance, Color.red);
    }
     
     void Check()
    {

        colliders = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        trail.Clear();
        target = null;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag("Player"))
            {
                target = colliders[i].gameObject.transform;
            }
            else if (colliders[i].gameObject.CompareTag("Trail"))
                trail.Add(colliders[i].gameObject.transform);  
        }
        if (trail.Count > 0)
            trailpos = trail[0];
        CheckRayThingy();
      
    }
    void CheckRayThingy()
    {
      
        bool isTrail = trail.Count > 0;
            bool isTarget = target;



        if (isTarget)
        {
            distance = target.position - transform.position;
            Ray(distance);
            if (hit && hit.collider.gameObject.CompareTag("Player"))
                Targeted(target);
        }
        if (isTrail)
        {
            distance = trailpos.position - transform.position;
            Ray(distance);
            if (hit && hit.collider.gameObject.CompareTag("Trail"))
            {

                Targeted(trailpos);
            }

        }
        if(!isTarget&&!isTrail)
            bandit.isFollowing = false;
    }
    void Targeted(Transform target)
    {
                     
            bandit.isFollowing = true;
        bandit.distance = distance;


    }
    void Ray(Vector2 distance)
    {

        hit = Physics2D.Raycast(transform.position, distance.normalized, radius, mask);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;   
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    /*
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
         
                isTarget = false;

        }
        if (collision.gameObject.CompareTag("Trail"))
        {
            trail.Remove(collision.gameObject);
        }
       
        
    }
    */

}
