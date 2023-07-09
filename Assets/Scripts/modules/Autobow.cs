using UnityEngine;

namespace modules
{
    public class Autobow : MonoBehaviour
    {
        private const float CooldownTime = 0.25f;
        private GameObject _projectile;
        private float timer;

        // Start is called before the first frame update
        private void Start()
        {
            _projectile = GetComponent<Sieger>().arrowPrefab;
            timer = CooldownTime;
        }

        // Update is called once per frame
        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0) shoot();
        }

        private void shoot()
        {
            Instantiate(_projectile, transform.position, Quaternion.identity);
            timer = CooldownTime;
        }
    }
}