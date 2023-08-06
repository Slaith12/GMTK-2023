using UnityEngine;

public class CrossbowBolt : MonoBehaviour
{
    [SerializeField] float lifetime = 1.5f;

    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}