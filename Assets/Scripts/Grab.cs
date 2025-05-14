using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grab : MonoBehaviour, Interactable
{
    public LayerMask mask;
    Vector3 mousePos;
    bool isPicked = false;
    State status;
    Camera cam;

    enum State
    {
        picked,
        unpicked,
    }
    void Start()
    {
        status = State.unpicked;
        cam= Camera.main;   
 
      
    }
    void Update()
    {
    
        for (int i = 0; i < 10; i++)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("grabbable"), i, isPicked);

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        switch (status)

        {
            case State.picked:
                Move(mousePos);
                isPicked = true;
                break;
            case State.unpicked:
                isPicked = false;
                break;
        }

    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (status == State.picked&&!Placeable())
                status = State.unpicked;
            else if(status==State.unpicked)
                status = State.picked;

        }

    }
    public void Move(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, 0);
    }
    bool Placeable()
    {
      
        return  Physics2D.OverlapBox(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 0, mask);

    }

}
