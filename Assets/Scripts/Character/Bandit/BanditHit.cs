using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditHit : MonoBehaviour
{
    BanditController controller;

  
    void Start()
    {
        controller =transform.parent.GetComponent<BanditController>();

        Physics2D.queriesHitTriggers = false;
    }
  
    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            controller.isFighting = true;
        if (other.CompareTag("Trail"))
            other.gameObject.GetComponent<Trail>().Delete();
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))        
            controller.isFighting = false;
        
    }
}
