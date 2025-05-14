using Pathfinding;
using UnityEngine;
public class enemyController : MonoBehaviour
{
    [SerializeField] Transform target;

    public float speed = 100f;
    public float nextWayPointDistance = 3f;

    Path path;
    Seeker seeker;
    Rigidbody2D rb;
    int currentWayPoint = 0;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }
    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    private void FixedUpdate()
    {
        if (path == null)
            return;
        if (currentWayPoint >= path.vectorPath.Count)
        {

            return;
        }
        
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if ((distance < nextWayPointDistance))

        {
            currentWayPoint++;
        }
        if (force.x >= 0)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (force.x <= 0)
            transform.localScale = new Vector3(1f, 1f, 1f);

    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

}
