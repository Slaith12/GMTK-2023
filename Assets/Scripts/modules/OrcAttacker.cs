using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcAttacker : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float offset;
    [SerializeField] GameObject proj;
    [SerializeField] float hitTime;
    [SerializeField] float speed;
    [SerializeField] float moveChangeTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("moveToAttack", moveChangeTime, moveChangeTime);
        InvokeRepeating("attack",hitTime,hitTime);
    }

    void attack() 
    {
        Instantiate(proj,this.transform);
    }

    void moveToAttack()
    {
        var targetlist = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        var target = targetlist[0].transform;
        var dist = Vector2.Distance(target.position, transform.position);
        foreach (var t in targetlist)
        {
            if (Vector2.Distance(t.transform.position, transform.position) < dist)
            {
                dist = Vector2.Distance(t.transform.position, transform.position);
                target = t.transform;
            }
        }
        Vector2 targetPos = new Vector2(target.position.x + Random.Range(-offset, offset),
            target.position.y + Random.Range(-offset, offset));

        rb.velocity = (targetPos - (Vector2)transform.position ).normalized * speed;
    }


}
