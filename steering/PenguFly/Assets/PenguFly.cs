using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguFly : MonoBehaviour
{
    private OVRCameraRig ovrCameraRig;
    public float speed = 25.0f;
    public Vector2 Idirection;
    public Vector2 direction;
    void Start()
    {
        ovrCameraRig = GetComponentInParent<OVRCameraRig>();
        
        
        Quaternion IcontrollerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Quaternion IheadRotation = ovrCameraRig.transform.rotation;
        Quaternion ItiltRotation = Quaternion.Inverse(IheadRotation) * IcontrollerRotation;


        Idirection = new Vector2(ItiltRotation.x, ItiltRotation.y).normalized;
    }

    void Update()
    {
        Vector3 forwardDirection = ovrCameraRig.transform.forward;
        forwardDirection.y = 0f;
        forwardDirection.Normalize();


        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Quaternion headRotation = ovrCameraRig.transform.rotation;
        Quaternion tiltRotation = Quaternion.Inverse(headRotation) * controllerRotation;
       

        direction = new Vector2(tiltRotation.x, tiltRotation.y).normalized - Idirection;


        Debug.Log(direction);

        Vector2 thumbstickDirection = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        

        Vector3 movementDirection = forwardDirection * direction.y + Camera.main.transform.right * direction.x;
        movementDirection.Normalize();

        float Speed = direction.magnitude * speed;
        //Move the camera in the calculated direction
        ovrCameraRig.transform.position += movementDirection * Speed * Time.deltaTime;


        
    }
}
