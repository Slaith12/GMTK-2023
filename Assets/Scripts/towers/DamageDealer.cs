using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // Start is called before the first frame update
    public int damageAmount;

    [Tooltip("true if the object is a bullet that should destroy when it hits something, false if it shouldn't")]
    [SerializeField]
    private bool bullet;

    private Collider2D col;
    private Rigidbody2D rb;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject.GetComponent<HealthObject>();
        if (obj != null)
            if (CompareTag(obj.damageType))
            {
                obj.healthPoints -= damageAmount;
                if (bullet)
                    Destroy(gameObject);
            }
    }
}