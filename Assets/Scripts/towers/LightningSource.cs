using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSource : MonoBehaviour
{
    [SerializeField] GameObject lightningBolt;
    [SerializeField] float boltSpeed;

    void Awake()
    {
        GameObject bolt1 = Instantiate(lightningBolt, transform.position, Quaternion.Euler(Vector3.zero));
        bolt1.GetComponent<Rigidbody2D>().velocity = new Vector2(-boltSpeed, 0);
        GameObject bolt2 = Instantiate(lightningBolt, transform.position, Quaternion.Euler(new Vector3(0,0,90)));
        bolt2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -boltSpeed);
        GameObject bolt3 = Instantiate(lightningBolt, transform.position, Quaternion.Euler(Vector3.zero));
        bolt3.GetComponent<Rigidbody2D>().velocity = new Vector2(boltSpeed, 0);
        GameObject bolt4 = Instantiate(lightningBolt, transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));
        bolt4.GetComponent<Rigidbody2D>().velocity = new Vector2(0, boltSpeed);
        Destroy(gameObject);
    }
}
