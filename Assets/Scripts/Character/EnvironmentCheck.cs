using UnityEngine;

public class EnvironmentCheck : MonoBehaviour
{
    [HideInInspector] public LayerMask canWalk { private get; set; }
     public bool isTouching { get; private set; }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((canWalk & (1 << collision.gameObject.layer)) != 0)
            isTouching = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((canWalk & (1 << collision.gameObject.layer)) != 0)
            isTouching = false;
    }
}
