using UnityEngine;

public class CrossbowBolt : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float arrowForce;

    // Start is called before the first frame update
    private void Start()
    {
        var targetlist = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        var target = targetlist[0].transform;
        var dist = Vector2.Distance(target.position, transform.position);
        foreach (var t in targetlist)
            if (Vector2.Distance(t.transform.position, transform.position) < dist)
            {
                dist = Vector2.Distance(t.transform.position, transform.position);
                target = t.transform;
            }

        var rot = Mathf.Atan((transform.position.y - target.position.y) /
                             (transform.position.x - target.position.x)) * Mathf.Rad2Deg;
        if (target.position.x < transform.position.x)
        {
            rot = 270 - rot;
            if (target.position.y > transform.position.y)
                rot = 180 - Mathf.Abs(rot - 180);
        }

        rb.SetRotation(rot);

        rb.AddForce((target.transform.position - transform.position).normalized * arrowForce);

        Destroy(gameObject, 1.5f);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}