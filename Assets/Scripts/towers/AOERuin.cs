using UnityEngine;

public class AOERuin : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float attackTime;
    [SerializeField] private float radius;

    private void Start()
    {
        LeanTween.scale(gameObject, new Vector2(radius, radius), attackTime);
        Destroy(gameObject, attackTime);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}