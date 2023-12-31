using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrcAttacker : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private GameObject proj;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float speed;
    [SerializeField] private float moveChangeTime;
    private static float siegeProtectionRadius = 7;
    [HideInInspector] public static Transform siegeMachine;
    private static int numOrcs;
    private static Transform target;
    [HideInInspector] public static bool attacking;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("moveToAttack", moveChangeTime, moveChangeTime);
        InvokeRepeating("attack", attackCooldown, attackCooldown);
        numOrcs++;
    }

    private void attack()
    {
        if (target == null || target.tag == "Player")
            return;
        Instantiate(proj, transform);
    }

    private void moveToAttack()
    {
        if (!attacking)
            return;

        if (target == null) //if target is destroyed
            FindNewTarget();
        var targetPos = new Vector2(target.position.x + Random.Range(-offset, offset),
            target.position.y + Random.Range(-offset, offset));

        rb.velocity = (targetPos - (Vector2) transform.position).normalized * speed;
    }

    private void OnDestroy()
    {
        numOrcs--;
    }

    public static IEnumerator RepeatedlyChangeTarget()
    {
        while(numOrcs > 0)
        {
            FindNewTarget();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public static void Disperse()
    {
        target = null;
        attacking = false;
    }

    private static void FindNewTarget()
    {
        Tower[] towerList = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        List<Transform> targetList = new List<Transform>();
        foreach (Tower tower in towerList)
            targetList.Add(tower.transform);
        targetList.RemoveAll(tower => Vector2.Distance(tower.position, siegeMachine.position) > siegeProtectionRadius);
        if (targetList.Count == 0)
            target = siegeMachine;
        else if (!targetList.Contains(target))
        {
            targetList.Sort(delegate (Transform tower1, Transform tower2)
            {
                float dist1 = Vector2.Distance(tower1.position, siegeMachine.position);
                float dist2 = Vector2.Distance(tower2.position, siegeMachine.position);
                if (dist1 > dist2)
                    return 1;
                else
                    return -1;
            });
            target = targetList[0];
        }
    }
}