using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTower : MonoBehaviour
{
    [SerializeField] float cooldownTime;
    [SerializeField] GameObject projectile;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("shoot", cooldownTime/2, cooldownTime);

    }

    void shoot() 
    {
        Instantiate(projectile, this.transform);
    }
}
