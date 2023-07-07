using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObject : MonoBehaviour
{
    [SerializeField] int maxHealth;
    public int healthPoints;

    public string damageType;

    // Start is called before the first frame update
    void Start()
    {
        healthPoints = maxHealth;
    }

    private void Update()
    {
        if (healthPoints < 0)
            Destroy(this.gameObject);
    }




}
