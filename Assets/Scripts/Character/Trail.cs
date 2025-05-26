using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] float lifeTime;
    Transform _transform;
    Transform target;
    void Start()
    {

        _transform = transform;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("Delete", 0, lifeTime);
    }
     public void Delete()
    {
        _transform.position=target.position;
    }
   


}
