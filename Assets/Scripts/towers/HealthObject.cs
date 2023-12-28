using UnityEngine;

public class HealthObject : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Vector2 healthBarOffset;
    private Transform healthBar;
    [SerializeField] int healthPoints;

    public string damageType;
    [Space]
    [SerializeField] private AudioClip damageSound;
    [SerializeField] float damageMinVolume = 0.8f;
    [SerializeField] float damageMaxVolume = 0.9f;
    [SerializeField] float damageMinPitch = 0.9f;
    [SerializeField] float damageMaxPitch = 1.1f;
    [Space]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] float deathMinVolume = 0.8f;
    [SerializeField] float deathMaxVolume = 0.9f;
    [SerializeField] float deathMinPitch = 0.9f;
    [SerializeField] float deathMaxPitch = 1.1f;

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

    public void Damage(int damage)
    {
        healthPoints -= damage;
        if(healthPoints <= 0)
        {
            float volume = Random.Range(deathMinVolume, deathMaxVolume);
            float pitch = Random.Range(deathMinPitch, deathMaxPitch);
            AudioObject.Create(deathSound, volume, pitch);
            Destroy(gameObject);
        }
        else
        {
            float volume = Random.Range(damageMinVolume, damageMaxVolume);
            float pitch = Random.Range(damageMinPitch, damageMaxPitch);
            AudioObject.Create(damageSound, volume, pitch);
            healthBar.localScale = new Vector3((float)healthPoints / maxHealth, 1, 1);
            healthBar.parent.position = (Vector2)transform.position + healthBarOffset;
        }

    }

    private void OnDestroy()
    {
        Destroy(healthBar.parent.gameObject);
    }
}