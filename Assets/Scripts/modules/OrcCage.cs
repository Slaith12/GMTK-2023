using UnityEngine;

public class OrcCage : MonoBehaviour
{
    [SerializeField] private GameObject orc;

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
        for (var i = 0; i < sieger.attackOrcsAvailable; i++)
        {
            GameObject newOrc = Instantiate(orc, transform.position, Quaternion.identity);
            newOrc.GetComponent<OrcAttacker>().siegeMachine = sieger.transform;
        }
        sieger.attackOrcsAvailable = 0;
    }
}