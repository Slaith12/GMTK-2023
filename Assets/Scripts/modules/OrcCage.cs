using UnityEngine;

public class OrcCage : MonoBehaviour
{
    [SerializeField] private GameObject orc;
    [SerializeField] float siegeProtectionRadius = 5;
    public static float _siegeProtectionRadius;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SpawnOrcs()
    {
        var sieger = GetComponent<Sieger>();

        if (sieger.attackOrcsAvailable <= 0)
            return;
        _siegeProtectionRadius = siegeProtectionRadius; 
        for (var i = 0; i < sieger.attackOrcsAvailable; i++)
        {
            Instantiate(orc, transform.position, Quaternion.identity);
        }
        OrcAttacker.siegeMachine = sieger.transform;
        OrcAttacker.attacking = true;
        StartCoroutine(OrcAttacker.RepeatedlyChangeTarget());
        sieger.attackOrcsAvailable = 0;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        OrcAttacker.Disperse();
    }
}