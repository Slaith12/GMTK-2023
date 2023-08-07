using UnityEngine;

public class LightningBolt : MonoBehaviour
{

    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, 2f);
    }
}