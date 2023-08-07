using UnityEngine;

namespace modules
{
    public class Autobow : MonoBehaviour
    {
        public int numBows;
        [SerializeField] float cooldownTime = 0.25f;
        [SerializeField] float shotSpeed = 6;
        [SerializeField] float maxRange = 5;
        [SerializeField] float rangeDecrement = 0.75f;
        [SerializeField] GameObject _projectile;
        private float timer;

        // Start is called before the first frame update
        private void Start()
        {
            _projectile = GetComponent<Sieger>().arrowPrefab;
            timer = cooldownTime;
        }

        // Update is called once per frame
        private void Update()
        {
            if (numBows <= 0)
                return;
            timer -= Time.deltaTime;
            if (timer <= 0) Shoot();
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
            if (dist > maxRange)
                return;

            float rot = Mathf.Atan((target.position.y - transform.position.y) /
                                 (target.position.x - transform.position.x)) * Mathf.Rad2Deg;
            if (target.position.x < transform.position.x)
            {
                rot -= 180;
            }

            GameObject proj = Instantiate(_projectile, transform.position, Quaternion.Euler(Vector3.forward * rot));

            proj.GetComponent<Rigidbody2D>().velocity = (target.transform.position - transform.position).normalized * shotSpeed;

            int availableBows = (int)((maxRange - dist) / rangeDecrement) + 1;
            timer = cooldownTime / Mathf.Min(availableBows, numBows);
        }
    }
}