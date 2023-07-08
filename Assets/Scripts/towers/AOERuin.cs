using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOERuin : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField ]float attackTime;
    [SerializeField] float radius;

    void Start()
    {
        LeanTween.scale(this.gameObject, new Vector2(radius, radius), attackTime);
        Destroy(this.gameObject, attackTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
