using UnityEngine;
using System;
using System.IO;
public class headmovement : MonoBehaviour
{
    public float distance = 2.0f; // Distance from the user's head
    public float speed = 0.5f; // Speed of camera movement
    public Quaternion headRotation;
    public float tiltAngle;
    public float initial;
    public float angle;
    public float delta;
    
    public string current_location;
    public string filePath1;
    public string filePath2;
    

    private OVRCameraRig ovrCameraRig;

    private void Start()
    {
        headRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        initial = headRotation.eulerAngles.x;

        ovrCameraRig = GetComponentInParent<OVRCameraRig>();
        

        filePath1 = Directory.GetCurrentDirectory() + "\\" + "position.txt";
        filePath2 = Directory.GetCurrentDirectory() + "\\" + "orientation.txt";


        File.WriteAllText(filePath1, "");
        File.WriteAllText(filePath2, "");
    }
    void Update()
    {
        // Get the rotation of the Oculus VR headset
        //headRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        headRotation = ovrCameraRig.centerEyeAnchor.transform.rotation;
        tiltAngle = headRotation.eulerAngles.x;

        delta = Time.deltaTime;

        Vector3 forwardDirection = ovrCameraRig.transform.forward;
        forwardDirection.y = 0f;
        forwardDirection.Normalize();

        angle = Math.Abs(tiltAngle - initial);  
        if (tiltAngle > 5 && tiltAngle < 300)
        {
            // Move the camera forward using the tilt angle


            //Vector3 movementDirection = Camera.main.transform.right * headRotation.x;
            Vector3 movement = ovrCameraRig.transform.forward * Time.deltaTime * speed;
            ovrCameraRig.transform.position += movement;
            //ovrCameraRig.transform.position += movementDirection;
        }

        initial = headRotation.eulerAngles.x;


       

        string position = ovrCameraRig.transform.position.ToString("F2") + "\n";
        string orientation = ovrCameraRig.centerEyeAnchor.transform.rotation.ToString("F2")+ "\n";
        current_location = position;


        
        File.AppendAllText(filePath1, position);
        File.AppendAllText(filePath2, orientation);
        

    }
}