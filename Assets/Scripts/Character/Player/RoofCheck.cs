using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofCheck : MonoBehaviour
{
    public bool roof;
    [SerializeField] Vector2 dir;
    public void Update()
    {
        roof = isRoofed();
    }
    bool isRoofed()
    {
        return Physics2D.Raycast(transform.position, dir, .5f, LayerMask.GetMask("Ground"));
    }

}
