using UnityEngine;

public class RoofCheck : MonoBehaviour
{
    [HideInInspector] public bool roof { get; private set; }
    [SerializeField] Vector2 dir;
    public void Update()
    {
        roof = isRoofed();
    }
    bool isRoofed()
    {
        return Physics2D.Raycast(transform.position, dir, .5f, LayerMask.GetMask("Ground","grabbable"));
    }

}
