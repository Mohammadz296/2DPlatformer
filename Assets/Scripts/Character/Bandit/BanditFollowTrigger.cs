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
    Transform _transform;
    Vector2 distance;
    RaycastHit2D hit;
    Collider2D[] colliders=new Collider2D[15];
    List<Transform> trail = new List<Transform>();
    bool isTrail;
    bool isTarget;
    void Start()
    {
        _transform=transform;
        bandit = gameObject.transform.parent.gameObject.GetComponent<BanditController>();

        Physics2D.queriesHitTriggers = false;
        InvokeRepeating("Check", 0f, .5f);

    }

    
    void Check()
    {

            trail.Clear();
        int count = Physics2D.OverlapCircleNonAlloc(_transform.position, radius,colliders);
        target = null;
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].gameObject.CompareTag("Player"))
            {
                target = colliders[i].gameObject.transform;
            }
            else if (colliders[i].gameObject.CompareTag("Trail"))
                trail.Add(colliders[i].gameObject.transform);  
        }
        isTrail = trail.Count > 0;
        isTarget = target !=null;
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
    void Targeted(Vector2 target)
    {                    
            bandit.isFollowing = true;
        bandit.distance = target;
    }
    void TrailRay()
    {
        
            distance = trailpos.position - _transform.position;
            Ray(distance, LayerMask.GetMask("trail", "ground", "grabbable"));
            if (hit && hit.collider.gameObject.CompareTag("Trail"))
                Targeted(distance);
            else
                bandit.isFollowing = false;
        
    }
    void TargetRay()
    {
        distance = target.position - _transform.position;
        Ray(distance, LayerMask.GetMask("player", "ground", "grabbable"));
        if (hit&&hit.collider.gameObject.CompareTag("Player"))  
            Targeted(distance);
        else if (isTrail)
            TrailRay();   
        else
            bandit.isFollowing = false;
    }
    void Ray(Vector2 distance,LayerMask layer)
    {

        hit = Physics2D.Raycast(_transform.position, distance.normalized, radius, layer);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_transform)
        {
            Gizmos.DrawWireSphere(_transform.position, radius);

        }
        if(hit)
            Debug.DrawRay(_transform.position, distance.normalized * radius, Color.red);
        if(trailpos)
            Gizmos.DrawWireSphere(trailpos.position, .5f);
    }
}
