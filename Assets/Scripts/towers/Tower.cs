using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] float cooldownTime;
    [SerializeField] GameObject siegeMachine;
    [SerializeField] GameObject projectile;
    [SerializeField] int shots;
    

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("shoot", cooldownTime, cooldownTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void shoot() 
    {
        for (int i = 0; i < shots; i++)
        {
            Instantiate(projectile, this.transform);
        }
    }
}
