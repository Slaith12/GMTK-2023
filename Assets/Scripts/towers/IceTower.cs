using UnityEngine;

public class IceTower : MonoBehaviour
{
    [SerializeField] private float cooldownTime;
    [SerializeField] private GameObject projectile;


    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("shoot", cooldownTime / 2, cooldownTime);
    }

    private void shoot()
    {
        Instantiate(projectile, transform);
    }
}