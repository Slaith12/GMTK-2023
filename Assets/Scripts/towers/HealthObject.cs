using UnityEngine;

public class HealthObject : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Vector2 healthBarOffset;
    private Transform healthBar;
    public int healthPoints;

    public string damageType;

    private void Start()
    {
        Debug.Log("Init Health for " + gameObject.name);
        healthPoints = maxHealth;
        Transform baseHealthBar = Instantiate(healthBarPrefab).transform;
        baseHealthBar.position = (Vector2)transform.position + healthBarOffset;
        baseHealthBar.localScale = new Vector3(maxHealth / 50f, 0.12f, 1);
        healthBar = baseHealthBar.GetChild(0);
    }

    private void Update()
    {
        healthBar.localScale = new Vector3((float)healthPoints / maxHealth, 1, 1);
        healthBar.parent.position = (Vector2)transform.position + healthBarOffset;
        if (healthPoints < 0)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Destroy(healthBar.parent.gameObject);
    }
}