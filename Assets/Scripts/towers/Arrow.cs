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
        rb.SetRotation(
            Mathf.Atan((transform.position.y - player.position.y) / (transform.position.x - player.position.x)) *
            Mathf.Rad2Deg);
        rb.AddForce((player.position - transform.position).normalized * arrowForce);

        Destroy(gameObject, 1.5f);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}