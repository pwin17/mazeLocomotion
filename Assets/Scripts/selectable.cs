using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectable : MonoBehaviour
{
    private Color defaultColor;
    private Color highlightedColor = Color.red;
    private Renderer objectRenderer;


    void Start()
    {
        
        objectRenderer = GetComponent<Renderer>();
        defaultColor = objectRenderer.material.color;
        
    }

    
    public void Highlight()
    {
        
        if (objectRenderer.material.color == defaultColor)
        {
            objectRenderer.material.SetColor("_Color", highlightedColor);
        }
        else
        {
            objectRenderer.material.SetColor("_Color", defaultColor);
        }
    }

}



