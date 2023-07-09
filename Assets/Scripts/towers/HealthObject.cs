using UnityEngine;

public class HealthObject : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    public int healthPoints;

    public string damageType;

    // Start is called before the first frame update
    private void Start()
    {
        healthPoints = maxHealth;
    }

    private void Update()
    {
        if (healthPoints < 0)
            Destroy(gameObject);
    }
}