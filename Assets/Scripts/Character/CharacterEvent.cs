using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterEvent : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [SerializeField] GameObject forleap;
    [SerializeField]Transform wallCheck;
    [SerializeField] Transform wallCheck2;
    [SerializeField] GameObject dust;
    Vector3 startAttack;
    CapsuleCollider2D bc;
    Character character;
    Transform start;
    Transform start2;
    float range;
   public float facing=-1;
   
    bool draw;
    bool canTime = true;
    private void Start()
    {
        Physics2D.queriesHitTriggers = false;

        
        character = forleap.GetComponent<Character>();
        bc = forleap.GetComponent<CapsuleCollider2D>();
        start = transform.parent.Find("attackStart").transform;
        start2 = transform.parent.Find("attackStart2").transform;


    }
    
    void Hit()
    {
        
        range = character.range;
        List<Character> target = new List<Character>();
        if (facing==1)        
            startAttack = start.position;
        
        else  
            startAttack = start2.position;
        
        Vector3 center = new Vector3(startAttack.x + facing * range / 2, startAttack.y + bc.size.y / 2, transform.position.z);
        Vector2 size = new Vector2(range, bc.size.y);

       
        Physics2D.queriesHitTriggers = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center,size, 0, mask);
        
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.GetComponent<Character>())
                target.Add(hit.gameObject.GetComponent<Character>());
           
        }
        character.StartCoroutine(character.Attack(target));

        draw = true;
    }
    void Parry()
    {
        StartCoroutine(character.ParryTime());
    }
    public void flip()
    {
        transform.localScale= new Vector3(transform.localScale.x*-1f,transform.localScale.y,transform.localScale.z);
        facing = facing * -1f;
    }
    void DustSpawn()
    {
        float face = 1;
        if (facing == -1)
            face = -1;
        Transform temp=wallCheck;
        dust.transform.localScale = new Vector3(face, transform.localScale.y, transform.localScale.z);
        if (facing == -1)
            temp = wallCheck2;
        Instantiate(dust, temp.position, Quaternion.identity);

    }
    private void OnDrawGizmos()
    {
        
        if (draw)
        {
            Vector3 center = new Vector3(startAttack.x + facing * range / 2, startAttack.y + bc.size.y / 2, transform.position.z);
            Vector2 size = new Vector2(range, bc.size.y);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, size);
           if(canTime)
            StartCoroutine(StopDraw());
        }
    }
    IEnumerator StopDraw()
    {
        canTime = false;
        yield return new WaitForSeconds(.3f);
        canTime = true;
        draw = false;
    }
}
