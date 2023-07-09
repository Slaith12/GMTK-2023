using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private float timerVar;
    [SerializeField] private GameObject puddle;
    [SerializeField] private float speed;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        var degrees = Random.Range(0, 2 * Mathf.PI);
        int LR;
        rb.velocity = new Vector2(Mathf.Cos(degrees), Mathf.Sin(degrees)) * speed;

        timer += Random.Range(-2 * timerVar, timerVar);
        Destroy(gameObject, timer);
    }

    private void OnDestroy()
    {
        Instantiate(puddle, transform.position, Quaternion.identity);
    }
}