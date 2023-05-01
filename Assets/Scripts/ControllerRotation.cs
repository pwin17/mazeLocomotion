using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rightPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        float leftPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        if (leftPressed > 0.1f)
        {
            float rotationSpeed = -25.0f * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationSpeed);
        }
        else if (rightPressed > 0.1f)
        {
            float rotationSpeed = 25.0f * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationSpeed);
        }
    }
}
