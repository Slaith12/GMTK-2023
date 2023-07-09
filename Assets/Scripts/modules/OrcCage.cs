using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcCage : MonoBehaviour
{
    [SerializeField] int orcCount;
    [SerializeField] GameObject orc;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnOrcs()
    {
        for(int i = 0; i < orcCount; i++) 
        {
            Instantiate(orc, this.transform);
        }
    }
}
