using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float arrowForce;
    [SerializeField] private Transform player;

    // Start is called before the first frame update
    private void Start()
    {
        player = FindObjectOfType<Sieger>().transform;
        float rot = Mathf.Atan((player.position.y - transform.position.y) /
                                 (player.position.x - transform.position.x)) * Mathf.Rad2Deg;
        if (player.position.x < transform.position.x)
        {
            rot -= 180;
        }
        transform.eulerAngles = new Vector3(0, 0, rot);
        rb.AddForce((player.position - transform.position).normalized * arrowForce);

        Destroy(gameObject, 1.5f);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}