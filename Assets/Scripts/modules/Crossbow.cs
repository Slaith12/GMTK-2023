using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    [SerializeField] float cooldownTime;
    float timer;
    [SerializeField] GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        timer = cooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && Input.GetMouseButtonDown(0)) 
        {
            shoot();
        }
    }

    void shoot() 
    {
        Instantiate(projectile, this.transform);
        timer = cooldownTime;
    }

    
}
