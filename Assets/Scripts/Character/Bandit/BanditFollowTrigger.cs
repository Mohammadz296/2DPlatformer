using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BanditFollowTrigger : MonoBehaviour
{
    BanditController bandit;
 
    public LayerMask mask;
    Transform target;
    [SerializeField] float radius;
    Transform trailpos;
    Vector2 distance;
    RaycastHit2D hit;
    Collider2D[] colliders;
    List<Transform> trail = new List<Transform>();
    bool isTrail;
    bool isTarget;
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
        isTrail = trail.Count > 0;
        isTarget = target;
        if (isTrail)
            trailpos = trail[0];
        CheckRayThingy();
      
    }
    void CheckRayThingy()
    {
        if (isTarget)  
            TargetRay();
        else if ( isTrail)      
           TrailRay();     
        if(!isTarget&&!isTrail)
            bandit.isFollowing = false;
    }
    void Targeted(Transform target)
    {                    
            bandit.isFollowing = true;
        bandit.distance = distance;
    }
    void TrailRay()
    {
        distance = trailpos.position - transform.position;
        Ray(distance, LayerMask.GetMask("trail","ground","grabbable"));
        if (hit && hit.collider.gameObject.CompareTag("Trail"))  
            Targeted(trailpos);
        else
            bandit.isFollowing = false;
    }
    void TargetRay()
    {
        distance = target.position - transform.position;
        Ray(distance, LayerMask.GetMask("player", "ground", "grabbable"));
        if (hit && hit.collider.gameObject.CompareTag("Player"))  
            Targeted(target);
        else if (isTrail)
            TrailRay();   
        else
            bandit.isFollowing = false;
    }
    void Ray(Vector2 distance,LayerMask layer)
    {

        hit = Physics2D.Raycast(transform.position, distance.normalized, radius, layer);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
