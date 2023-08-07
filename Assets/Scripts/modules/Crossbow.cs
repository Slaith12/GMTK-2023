using System.Collections.Generic;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    [SerializeField] float cooldownTime = 0.5f;
    [SerializeField] float shotSpeed = 6;
    [SerializeField] GameObject _projectile;
    public List<float> timers;

    private void Awake()
    {
        timers = new List<float>();
    }

    private void Start()
    {
        _projectile = GetComponent<Sieger>().arrowPrefab;
    }

    // Update is called once per frame
    private void Update()
    {
        bool tryShoot = Input.GetMouseButtonDown(0);
        for(int i = 0; i < timers.Count; i++)
        {
            if(timers[i] > 0)
            {
                timers[i] -= Time.deltaTime;
            }
            else if(tryShoot)
            {
                Shoot();
                timers[i] = cooldownTime;
                tryShoot = false;
            }
        }
    }

    private void Shoot()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float rot = Mathf.Atan((mousePos.y - transform.position.y) /
                             (mousePos.x - transform.position.x)) * Mathf.Rad2Deg;
        if (mousePos.x < transform.position.x)
        {
            rot -= 180;
        }
        GameObject proj = Instantiate(_projectile, transform.position, Quaternion.Euler(Vector3.forward*rot));

        proj.GetComponent<Rigidbody2D>().velocity = (mousePos - (Vector2)transform.position).normalized * shotSpeed;
    }
}