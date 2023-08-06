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
        //find closest tower
        Tower[] targetlist = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        if (targetlist.Length == 0)
            return;
        Transform target = targetlist[0].transform;
        float dist = Vector2.Distance(target.position, transform.position);
        foreach (Tower t in targetlist)
        {
            if (Vector2.Distance(t.transform.position, transform.position) < dist)
            {
                dist = Vector2.Distance(t.transform.position, transform.position);
                target = t.transform;
            }
        }

        float rot = Mathf.Atan((target.position.y - transform.position.y) /
                             (target.position.x - transform.position.x)) * Mathf.Rad2Deg;
        if (target.position.x < transform.position.x)
        {
            rot -= 180;
        }
        GameObject proj = Instantiate(_projectile, transform.position, Quaternion.Euler(Vector3.forward*rot));

        proj.GetComponent<Rigidbody2D>().velocity = (target.transform.position - transform.position).normalized * shotSpeed;
    }
}