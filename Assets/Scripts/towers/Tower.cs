using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float cooldownTime;
    [SerializeField] private GameObject projectile;
    [SerializeField] private int shots;

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