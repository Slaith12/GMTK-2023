using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowBolt : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;
    [SerializeField] float arrowForce;

    // Start is called before the first frame update
    void Start()
    {
        Tower[] targetlist = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        Transform target = targetlist[0].transform;
        float dist = Vector2.Distance(target.position, transform.position);
        foreach (Tower t in targetlist) 
        {
            if (Vector2.Distance(t.transform.position, transform.position) < dist) 
            {
                dist = Vector2.Distance(t.transform.position, transform.position);
                target = t.transform;
            }
        }
        float rot = Mathf.Atan((transform.position.y - target.position.y) /
            (transform.position.x - target.position.x)) * Mathf.Rad2Deg;
        if(target.position.x < transform.position.x)
        {
            rot = 270 - rot;
            if (target.position.y > transform.position.y)
                rot = 180 - Mathf.Abs(rot - 180);
        }
        rb.SetRotation(rot);
        
        rb.AddForce((target.transform.position - transform.position).normalized * arrowForce);

        Destroy(this.gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
