using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float arrowForce;
    [SerializeField] Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<SiegeMachine>().transform;
        rb.SetRotation(Mathf.Atan(( transform.position.y - player.position.y) / (transform.position.x - player.position.x)) * Mathf.Rad2Deg);
        rb.AddForce((player.position - transform.position).normalized * arrowForce);
        
        Destroy(this.gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
