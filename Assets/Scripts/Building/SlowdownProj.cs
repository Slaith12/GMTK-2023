using UnityEngine;

public class SlowdownProj : MonoBehaviour
{
    [SerializeField] private float slowdownAmount;
    [SerializeField] private float lifeTime;
    private Vector2 objspeed;
    private float siegerSpeed;
    private float siegerTurnSpeed;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("object entered");
        var rb = col.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            objspeed = rb.velocity;
            //rb.velocity *= slowdownAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("object exited");
        var rb = col.gameObject.GetComponent<Rigidbody2D>();
        //if (rb != null) rb.velocity = objspeed;
    }

}