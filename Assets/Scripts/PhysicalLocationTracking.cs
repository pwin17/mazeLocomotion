using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhysicalLocationTracking : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("----------------Boundary Trigger Entering-------------");
        Debug.Log("trigger object " + other.name);
        IsHitting.isBoundary = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("----------------Boundary Trigger Exiting-------------");
        IsHitting.isBoundary = false;
    }

}
