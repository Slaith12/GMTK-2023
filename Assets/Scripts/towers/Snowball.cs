using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] float timer;
    [SerializeField] float timerVar;
    [SerializeField] GameObject puddle;
    [SerializeField] float speed;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        float degrees = Random.Range(0, 2*Mathf.PI);
        int LR;
        rb.velocity = new Vector2(Mathf.Cos(degrees), Mathf.Sin(degrees)) * speed;

        timer += Random.Range(-2 * timerVar, timerVar);
        Destroy(gameObject, timer);
    }

    private void OnDestroy()
    {
        Instantiate(puddle, transform.position, Quaternion.identity);
    }
}
