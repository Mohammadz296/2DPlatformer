using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void Start()
    {
        Physics2D.queriesHitTriggers = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInChildren<CharacterManager>())
            collision.GetComponentInChildren<CharacterManager>().Death();
    }
}
