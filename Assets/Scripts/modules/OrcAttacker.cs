using UnityEngine;

public class OrcAttacker : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private GameObject proj;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float speed;
    [SerializeField] private float moveChangeTime;
    [SerializeField] float siegeProtectionRadius;
    [HideInInspector] public Transform siegeMachine;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("moveToAttack", moveChangeTime, moveChangeTime);
        InvokeRepeating("attack", attackCooldown, attackCooldown);
    }

    private void attack()
    {
        Instantiate(proj, transform);
    }

    private void moveToAttack()
    {
        Tower[] targetlist = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        Transform target = targetlist[0].transform;
        float dist = Vector2.Distance(target.position, transform.position);
        bool protecting = Vector2.Distance(target.position, siegeMachine.position) < siegeProtectionRadius;
        foreach (Tower t in targetlist)
            if (Vector2.Distance(t.transform.position, transform.position) < dist)
            {
                bool wouldProtect = Vector2.Distance(t.transform.position, siegeMachine.position) < siegeProtectionRadius;
                if(protecting && ! wouldProtect)
                    continue;
                dist = Vector2.Distance(t.transform.position, transform.position);
                target = t.transform;
                protecting = wouldProtect;
            }

        var targetPos = new Vector2(target.position.x + Random.Range(-offset, offset),
            target.position.y + Random.Range(-offset, offset));

        rb.velocity = (targetPos - (Vector2) transform.position).normalized * speed;
    }
}