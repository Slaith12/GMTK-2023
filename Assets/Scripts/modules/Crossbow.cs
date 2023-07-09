using UnityEngine;

public class Crossbow : MonoBehaviour
{
    [SerializeField] private float cooldownTime;
    [SerializeField] private GameObject projectile;
    private float timer;

    // Start is called before the first frame update
    private void Start()
    {
        timer = cooldownTime;
    }

    // Update is called once per frame
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && Input.GetMouseButtonDown(0)) shoot();
    }

    private void shoot()
    {
        Instantiate(projectile, transform);
        timer = cooldownTime;
    }
}