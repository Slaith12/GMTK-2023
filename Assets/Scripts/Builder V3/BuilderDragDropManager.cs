using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuilderDragDropManager : MonoBehaviour
{
    private VisualElement currentlyDragged;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Grab();
        }
        if(Input.GetMouseButtonUp(0))
        {
            Release();
        }
        if(currentlyDragged != null)
        {
            Drag();
        }
    }

    private void Grab()
    {

    }

    private void Release()
    {
        
    }

    private void Drag()
    {
        
    }
}
