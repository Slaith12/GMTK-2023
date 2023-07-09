using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    [SerializeField] private float boltspeed;

    [SerializeField] private Rigidbody2D rb;

    //0: left; 1: down; 2: right; 3: up
    private int dir;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dir = Random.Range(0, 4);
        switch (dir)
        {
            case 0: //left
                rb.velocity = new Vector2(-boltspeed, 0);
                break;
            case 1: //down
                rb.velocity = new Vector2(0, -boltspeed);
                rb.SetRotation(90);
                break;
            case 2: //right
                rb.velocity = new Vector2(boltspeed, 0);
                break;
            case 3: //up
                rb.velocity = new Vector2(0, boltspeed);
                rb.SetRotation(90);
                break;
        }

        Destroy(gameObject, 2f);
    }
}