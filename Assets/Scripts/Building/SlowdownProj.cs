using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowdownProj : MonoBehaviour
{
    Vector2 objspeed;
    [SerializeField] float slowdownAmount;
    [SerializeField] float lifeTime;

    void Start()
    {
        Destroy(gameObject,lifeTime);
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null) 
        {
            objspeed = rb.velocity;
            rb.velocity *= slowdownAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = objspeed;
        }
    }
}
