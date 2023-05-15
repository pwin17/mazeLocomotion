using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR;
using static GrabObject;
using System;
using UnityEngine.SceneManagement;


public class menu_contorller : MonoBehaviour
{
    // Variable to store interacted object
    public GameObject selectedObject;
    private GameObject grabbedObject;

    public float rayLength = 100f;
    public LayerMask layerMask;
    public Color rayColor1 = Color.green;
    public Color rayColor2 = Color.white;
    private LineRenderer lineRenderer;

    public Vector3 relativePosition;
    public Quaternion relativeRotation;
    public float pressedValue;
    public bool Value;


    public string name;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = rayColor1;
        lineRenderer.endColor = rayColor1;
    }

    // Update is called once per frame
    void Update()
    {   


        // Get the button press and call CastRay() if pressed
        if (GetButtonAPress())
        {
            CastRay();
            
        }

        
    }


    private void CastRay()
    {
        Console.WriteLine("1");
        RaycastHit hit;
        



        if (Physics.Raycast(GetPosition(), GetPointingDir(), out hit))
        {
            
            
            selectedObject = hit.collider.gameObject;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.material.color = rayColor2;


            selectable selectableObject = selectedObject.GetComponent<selectable>();

            name = selectableObject.name;

            if (selectableObject != null)
            {
                if (name == "Steer"){
                    SceneManager.LoadScene("SteeringScene", LoadSceneMode.Single); 
                    
                }

                if (name == "RDW"){
                    SceneManager.LoadScene("RDWScene", LoadSceneMode.Single); 
                }

                if (name == "Hybrid"){
                    SceneManager.LoadScene("HybridScene", LoadSceneMode.Single); 
                }
            }
        
            Debug.Log(name);


            if (selectableObject != null)
            {
                selectableObject.Highlight();

                
            }



            

        }
        else
        {
            // Set selectedObject to null if no object is hit
            selectedObject = null;


            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * rayLength);
            lineRenderer.material.color = rayColor1;

        }
    }


    private Vector3 GetPointingDir()
    {
        return transform.forward;
        
    }

   
    private Vector3 GetPosition()
    {
        return transform.position;
    }

    
    private bool GetButtonAPress()
    {

        return OVRInput.GetUp(OVRInput.Button.One);
    }

    private bool GetButtonBPress()
    {

        return OVRInput.GetUp(OVRInput.Button.Two);
    }
    
    

}





