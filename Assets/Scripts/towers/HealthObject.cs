using UnityEngine;

public class HealthObject : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Vector2 healthBarOffset;
    private Transform healthBar;
    [SerializeField] int healthPoints;

    public string damageType;
    [SerializeField] float damageFlashHoldDuration = 0.1f;
    [SerializeField] float damageFlashTotalDuration = 0.25f;
    private float damageFlashTimer;
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

    private new SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        Debug.Log("Init Health for " + gameObject.name);
        healthPoints = maxHealth;
        Transform baseHealthBar = Instantiate(healthBarPrefab).transform;
        baseHealthBar.position = (Vector2)transform.position + healthBarOffset;
        baseHealthBar.localScale = new Vector3(maxHealth / 50f, 0.12f, 1);
        healthBar = baseHealthBar.GetChild(0);
        damageFlashTimer = damageFlashTotalDuration;
    }

    private void Update()
    {
        if(damageFlashTimer < damageFlashTotalDuration)
        {
            damageFlashTimer += Time.deltaTime;
            float intensity = (damageFlashTimer - damageFlashHoldDuration) / (damageFlashTotalDuration - damageFlashHoldDuration);
            if (intensity > 1)
                intensity = 1;
            else if (intensity < 0)
                intensity = 0;
            renderer.color = new Color(1, intensity, intensity);
        }
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
            renderer.color = Color.red;
            damageFlashTimer = 0;
        }

    }

    private void OnDestroy()
    {
        Destroy(healthBar.parent.gameObject);
    }
}