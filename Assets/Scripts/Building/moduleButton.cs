using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moduleButton : MonoBehaviour
{
    public GameObject component;

    private void Update()
    {
        GetComponent<Image>().sprite = component.GetComponent<SpriteRenderer>().sprite;
    }
    public void pressComponent()
    {
        GetComponentInParent<siegeBuilder>().chosenComponent = component;
    }
}