using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class siegeBuilder : MonoBehaviour
{
    public GameObject chosenComponent;
    public GameObject cursor;
    public bool TheresKing;
    private void Start()
    {
        TheresKing = false;
    }
    private void Update()
    {
        if (chosenComponent != null)
        {
            cursor.GetComponent<SpriteRenderer>().sprite = chosenComponent.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            cursor.GetComponent<SpriteRenderer>().sprite = null;
        }
        cursor.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
    }
}