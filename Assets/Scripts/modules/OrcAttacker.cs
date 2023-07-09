using UnityEngine;

public class OrcAttacker : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private GameObject proj;
    [SerializeField] private float hitTime;
    [SerializeField] private float speed;
    [SerializeField] private float moveChangeTime;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("moveToAttack", moveChangeTime, moveChangeTime);
        InvokeRepeating("attack", hitTime, hitTime);
    }

    private void attack()
    {
        Instantiate(proj, transform);
    }

    private void moveToAttack()
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

        var targetPos = new Vector2(target.position.x + Random.Range(-offset, offset),
            target.position.y + Random.Range(-offset, offset));

        rb.velocity = (targetPos - (Vector2) transform.position).normalized * speed;
    }
}