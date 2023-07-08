using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // Start is called before the first frame update
    public int damageAmount;
    [Tooltip("true if the object is a bullet that should destroy when it hits something, false if it shouldn't")]
    [SerializeField] bool bullet; //whether or not the player
    Collider2D col;
    Rigidbody2D rb;
    
    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthObject obj = collision.gameObject.GetComponent<HealthObject>();
        if (obj != null) 
        {
            if (this.CompareTag(obj.damageType))
            {
                obj.healthPoints -= damageAmount;
                if (bullet)
                    Destroy(this.gameObject);
            }
        }
        
        
    }



}
