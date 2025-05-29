using UnityEngine;

public class Tower : Activatable
{
    [SerializeField] private float cooldownTime;
    [SerializeField] private GameObject projectile;
    [SerializeField] private int shots;
    [HideInInspector] public HealthObject health;

    private void Awake()
    {
        health = GetComponent<HealthObject>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("shoot", cooldownTime, cooldownTime);
    }

    private void shoot()
    {
        for (var i = 0; i < shots; i++) Instantiate(projectile, transform);
    }
}