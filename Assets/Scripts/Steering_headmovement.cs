using UnityEngine;
using System;
using System.IO;
public class Steering_headmovement : MonoBehaviour
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
    
    public bool enable = true;


    private OVRCameraRig ovrCameraRig;

    // Sanjaya additions
    public Vector3 forwardDirection;
    public Vector3 lookingDirection;

    private void Start()
    {
        headRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        

        ovrCameraRig = GetComponentInParent<OVRCameraRig>();
        

        filePath1 = Directory.GetCurrentDirectory() + "\\" + "position.txt";
        filePath2 = Directory.GetCurrentDirectory() + "\\" + "orientation.txt";


        File.WriteAllText(filePath1, "");
        File.WriteAllText(filePath2, "");
    }
    void Update()
    {

        GetButtonAPress();

        if (enable){
            headRotation = ovrCameraRig.centerEyeAnchor.transform.rotation;
            tiltAngle = headRotation.eulerAngles.x;

            delta = Time.deltaTime;

            forwardDirection = ovrCameraRig.transform.forward;
            forwardDirection.y = 0f;
            forwardDirection.Normalize();


        
            Transform centerEyeAnchor = ovrCameraRig.centerEyeAnchor;
            lookingDirection = centerEyeAnchor.forward;
            lookingDirection.y = 0f;
            lookingDirection.Normalize();
            


            
            if (tiltAngle > 5 && tiltAngle < 300)
            {
                // Move the camera forward using the tilt angle


                Vector3 movement = lookingDirection * Time.deltaTime * speed*tiltAngle/10;
                ovrCameraRig.transform.position += movement;
                
            }

            

            string position = ovrCameraRig.transform.position.ToString("F2") + "\n";
            string orientation = ovrCameraRig.centerEyeAnchor.transform.rotation.ToString("F2")+ "\n"; // should be transform.forward // also please do euler instead of quaternion
            current_location = position;


            
            File.AppendAllText(filePath1, position);
            File.AppendAllText(filePath2, orientation);
        }
    }


    private void GetButtonAPress()
    {
       
        bool v = OVRInput.GetDown(OVRInput.Button.One);
        if (v){
            if (enable){
            enable = false;
            
            }
            else{
                enable = true;
            }

        }

        


    }
}