using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    [SerializeField] Sieger s;
    [Tooltip("This is a percent - like .05 or .1 not 1.05")]
    [SerializeField] float speedboostpercent;
    private float baseMoveSpeed;
    private float baseTurnSpeed;


    // Start is called before the first frame update
    void Start()
    {
        baseMoveSpeed = s.movementSpeed;
        baseTurnSpeed = s.turningSpeed;

        s.movementSpeed += baseMoveSpeed * speedboostpercent;
        s.turningSpeed += baseTurnSpeed * speedboostpercent;
    }
}
