using UnityEngine;

public class OrcCage : MonoBehaviour
{
    [SerializeField] private int orcCount;
    [SerializeField] private GameObject orc;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void spawnOrcs()
    {
        for (var i = 0; i < orcCount; i++) Instantiate(orc, transform);
    }
}