using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    //0: left; 1: down; 2: right; 3: up
    int dir;
    [SerializeField] float boltspeed;
    [SerializeField] Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dir = Random.Range(0, 4);
        switch(dir)
        {
            case 0: //left
                rb.velocity = new Vector2(-boltspeed,0);
                break;
            case 1: //down
                rb.velocity = new Vector2(0,-boltspeed);
                rb.SetRotation(90);
                break;
            case 2: //right
                rb.velocity = new Vector2(boltspeed, 0);
                break;
            case 3: //up
                rb.velocity = new Vector2(0, boltspeed);
                rb.SetRotation(90);
                break;
        }

        Destroy(this.gameObject, 2f);
    }

}
