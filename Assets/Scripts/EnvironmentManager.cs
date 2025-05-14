using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public float yVel = 0;
    public float xVel = 0;
    EnvironmentController[] environmentControllers;
    public void Start()
    {
        environmentControllers = GetComponentsInChildren<EnvironmentController>();
    }
    public void Update()
    {
        foreach (EnvironmentController e in environmentControllers)
            e.Move(xVel, yVel);

    }


}
