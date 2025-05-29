using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace modules
{
    public class Autobow : MonoBehaviour
    {
        public int numBows;
        [SerializeField] float cooldownTime = 0.25f;
        [SerializeField] float shotSpeed = 6;
        [SerializeField] float maxRange = 5;
        //[SerializeField] float rangeDecrement = 0.75f;
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
            //find closest towers
            List<Tower> targetList = new List<Tower>(FindObjectsByType<Tower>(FindObjectsSortMode.None));
            targetList.RemoveAll(tower => Vector2.Distance(tower.transform.position, this.transform.position) > maxRange);
            targetList.Sort(delegate (Tower tower1, Tower tower2)
            {
                float dist1 = Vector2.Distance(tower1.transform.position, this.transform.position);
                float dist2 = Vector2.Distance(tower2.transform.position, this.transform.position);
                if (dist1 > dist2)
                    return 1;
                else
                    return -1;
            });
            targetList.Sort(delegate (Tower tower1, Tower tower2)
            {
                if (tower1.health.healthPoints > tower2.health.healthPoints)
                    return 1;
                else
                    return -1;
            });
            for(int i = 0; i < Mathf.Min(numBows, targetList.Count); i++)
            {
                Transform target = targetList[i].transform;

                float rot = Mathf.Atan((target.position.y - transform.position.y) /
                                     (target.position.x - transform.position.x)) * Mathf.Rad2Deg;
                if (target.position.x < transform.position.x)
                {
                    rot -= 180;
                }

                GameObject proj = Instantiate(_projectile, transform.position, Quaternion.Euler(Vector3.forward * rot));

                proj.GetComponent<Rigidbody2D>().velocity = (target.transform.position - transform.position).normalized * shotSpeed;
            }
            timer = cooldownTime;
        }
    }
}