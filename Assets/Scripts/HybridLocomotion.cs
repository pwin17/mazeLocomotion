using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HybridLocomotion : MonoBehaviour
{
    //  PUBLIC VARIABLE
    public Transform centerEyeAnchor;
    public string filePath1;
    public string filePath2;
    //  STEERING VARIABLES
    bool isSteering = false;
    bool isRDW = true;
    private Quaternion Steering_headRotation;
    private float Steering_tiltAngle;
    private Vector3 Steering_forwardDirection;
    private Vector3 Steering_lookingDirection;
    private bool Steering_enable;
    // RDW VARIABLES
    static float PE_SHORT_DIMENSION = 4.3f;
    static float PE_LONG_DIMENSION = 6.125F;
    float ELLIPSE_Y_RADIUS = PE_LONG_DIMENSION / 2.0f;
    float ELLIPSE_X_RADIUS = PE_SHORT_DIMENSION / 2.0f;
    public float RDW_walkingThreshold = 0.3f;
    private Vector3 RDW_previousHeadPosition;
    public float RDW_ConstantTranslationGain = 5.0f;
    GameObject resetParentObj, VE;
    GameObject northBorder, eastBorder, westBorder, southBorder;
    bool isResetting = false;
    // Start is called before the first frame update
    void Start()
    {
        ;
        filePath1 = Directory.GetCurrentDirectory() + "\\" + "position.txt";
        filePath2 = Directory.GetCurrentDirectory() + "\\" + "orientation.txt";

        File.WriteAllText(filePath1, "");
        File.WriteAllText(filePath2, "");

        resetParentObj = new GameObject("Reset Parent Obj"); 
        VE = GameObject.Find("VE");
        // CenterEnv();
    }

    // Update is called once per frame
    void Update()
    {
        bool buttonPressed = OVRInput.GetDown(OVRInput.Button.Two);
        if (buttonPressed)
        {
            isRDW = !isRDW;
            isSteering = !isSteering;

        }
        if (isRDW)
        {
            Debug.Log("RDW HAPPENING");
            if (OVRInput.GetDown(OVRInput.Button.One))
                isResetting = !isResetting;if (IsHitting.isBoundary)
            {
                resetParentObj.transform.position = centerEyeAnchor.position;
                resetParentObj.transform.forward = new Vector3(centerEyeAnchor.forward.x, 0.0f, centerEyeAnchor.forward.z);
                VE.transform.parent = resetParentObj.transform;
            }
            else if (isResetting)
            {
                Debug.Log("resetting with a");
                resetParentObj.transform.position = centerEyeAnchor.position;
                resetParentObj.transform.forward = new Vector3(centerEyeAnchor.forward.x, 0.0f, centerEyeAnchor.forward.z);
                VE.transform.parent = resetParentObj.transform;
            }
            else
            {
                    VE.transform.parent = null;
                Vector3 centerEyeAnchorDelta = centerEyeAnchor.position - RDW_previousHeadPosition;
                
                if (Mathf.Abs(centerEyeAnchorDelta.z) > RDW_walkingThreshold)
                {
                    Debug.Log("---------------");
                    Debug.Log("Translation Gain Z happens here");
                    Vector3 currOffset = ApplyTranslationGain(Vector3.forward); //check
                    // RecenterBoundary(currOffset);
                    Debug.Log("---------------");
                }
                else if (Mathf.Abs(centerEyeAnchorDelta.x) > RDW_walkingThreshold)
                {
                    Debug.Log("---------------");
                    Debug.Log("Translation Gain X happens here");
                    Vector3 currOffset = ApplyTranslationGain(Vector3.right);
                    Debug.Log("---------------");
                }
            }
            RDW_previousHeadPosition = centerEyeAnchor.position; // check
        }
        else
        {
            Debug.Log("STEERING HAPPENING");
            
            if (OVRInput.GetDown(OVRInput.Button.One))
                Steering_enable = !Steering_enable;
            
            if (Steering_enable)
            {
                Steering_headRotation = centerEyeAnchor.rotation;
                Steering_tiltAngle = Steering_headRotation.eulerAngles.x;

                float delta = Time.deltaTime;

                Steering_forwardDirection = transform.forward;
                Steering_forwardDirection.y = 0f;
                Steering_forwardDirection.Normalize();

                Steering_lookingDirection = centerEyeAnchor.forward;
                Steering_lookingDirection.y = 0f;
                Steering_lookingDirection.Normalize();

                    if (Steering_tiltAngle > 5 && Steering_tiltAngle < 300)
                {
                    // Move the camera forward using the tilt angle


                    Vector3 movement = Steering_lookingDirection * Time.deltaTime * 0.5f * Steering_tiltAngle/10;
                    transform.position += movement;
                    
                }
                string position = transform.position.ToString("F2") + "\n";
                string orientation = centerEyeAnchor.transform.rotation.ToString("F2")+ "\n"; // should be transform.forward // also please do euler instead of quaternion

                File.AppendAllText(filePath1, position);
                File.AppendAllText(filePath2, orientation);
            }
                
        }
        
    }
    private Vector3 ApplyTranslationGain(Vector3 appliedDirection)
    {
        Vector3 userFacingDirection = centerEyeAnchor.forward;
        userFacingDirection.Normalize();
        userFacingDirection = Vector3.Scale(userFacingDirection, appliedDirection); // only take the direction user is moving in
        Debug.Log("User Facing Direction" + userFacingDirection);

        Vector3 movementDirection = userFacingDirection;
        float translationGain = IsHitting.isWall ? 0.0f: RDW_ConstantTranslationGain;
        movementDirection *= translationGain;
        Vector3 newPosition = movementDirection;
        Vector3 prevPosition =transform.position;
        transform.position += newPosition * Time.deltaTime;
        Vector3 positionOffset = transform.position - prevPosition;
        return positionOffset;
    }
}
