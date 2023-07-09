using UnityEngine;

public class OrcRock : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float throwForce;
    [SerializeField] private float lifetime;


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

        rb.AddForce((target.transform.position - transform.position).normalized * throwForce);

        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}