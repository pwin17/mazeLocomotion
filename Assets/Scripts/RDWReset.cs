using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDWReset : MonoBehaviour
{
    bool isResetting = false;
    bool drawPhysEnvMarkers = true;
    // GameObject trackingSpace;
    GameObject resetParentObj;
    GameObject hmd;
    GameObject VE;

    GameObject northBorder;
    GameObject eastBorder;
    GameObject westBorder;
    GameObject southBorder;

    static float PE_SHORT_DIMENSION = 4.3f;
    static float PE_LONG_DIMENSION = 6.125f;
    // static float PE_SHORT_DIMENSION = 4.0f;
    // static float PE_LONG_DIMENSION = 6.0f   ;

    float ELLIPSE_Y_RADIUS = PE_LONG_DIMENSION / 2.0f;
    float ELLIPSE_X_RADIUS = PE_SHORT_DIMENSION / 2.0f;
    Vector3 previous_position;
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        resetParentObj = new GameObject("Reset Parent Obj"); 
        hmd = GameObject.Find("CenterEyeAnchor");  
        VE = GameObject.Find("VE");
        // trackingSpace = GameObject.Find("TrackingSpace");
        Debug.Log("starting position "+ hmd.transform.position);
        
        CenterEnv();
        previous_position = hmd.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (IsHitting.isBoundary)
        {
            resetParentObj.transform.position = hmd.transform.position;
            resetParentObj.transform.forward = new Vector3(hmd.transform.forward.x, 0.0f, hmd.transform.forward.z);
            VE.transform.parent = resetParentObj.transform;
        }
        else
        {
            VE.transform.parent = null;
        }
        if (IsHitting.isBoundary)
        {
            resetParentObj.transform.forward = new Vector3(hmd.transform.forward.x, 0.0f, hmd.transform.forward.z);
            resetParentObj.transform.position = hmd.transform.position;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            IsHitting.isBoundary = !IsHitting.isBoundary;
        }
        // if (Input.GetKeyUp(KeyCode.R))
        // {
        //     isResetting = !isResetting;
        //     if (isResetting)
        //     {
        //         resetParentObj.transform.position = hmd.transform.position;
        //         resetParentObj.transform.forward = new Vector3(hmd.transform.forward.x, 0.0f, hmd.transform.forward.z);
        //         VE.transform.parent = resetParentObj.transform;
        //     }
        //     else
        //     {
        //         VE.transform.parent = null;
        //     }
        // }
        // if (isResetting)
        // {
        //     resetParentObj.transform.forward = new Vector3(hmd.transform.forward.x, 0.0f, hmd.transform.forward.z);
        //     resetParentObj.transform.position = hmd.transform.position;
        // }
        // if (IsHitting.isWall)
        // {
        //     hmd.transform.position = previous_position;
        // }
        // else
        // {
        //     previous_position = hmd.transform.position;
        // }
    }

    void CenterEnv()
    {
        Vector3 curPos = hmd.transform.position;
        Debug.Log("=================================================");
        Debug.Log("current position " + curPos);
        Vector3 curForward = hmd.transform.forward;
        Debug.Log("current forward " + curForward);
        float headingTheta = Mathf.Atan2(curForward.z, curForward.x);
        float offsetToNorth = Vector2.SignedAngle(new Vector2(curForward.x, curForward.z), new Vector2(0.0f, 1.0f));
        float newAngle = headingTheta - (Mathf.PI/2.0f);

        // Align the VE
        // VE.transform.position = curPos + new Vector3(0.0f, -curPos.y, 0.0f);
        // VE.transform.forward = new Vector3(curForward.x, 0.0f, curForward.z);

        // Make the PE border
        Destroy(northBorder);
        Destroy(eastBorder);
        Destroy(southBorder);
        Destroy(westBorder);
        northBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        northBorder.transform.position = new Vector3(curPos.x, 0.0f, curPos.z) + (new Vector3(Mathf.Cos(headingTheta), 0.0f, Mathf.Sin(headingTheta)) * ELLIPSE_Y_RADIUS);
        northBorder.transform.localScale = new Vector3(PE_SHORT_DIMENSION, 0.5f, 0.1f);
        northBorder.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        northBorder.AddComponent<BoxCollider>();
        northBorder.GetComponent<BoxCollider>().isTrigger = true;
        northBorder.AddComponent<PhysicalLocationTracking>();


        southBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        southBorder.transform.position = new Vector3(curPos.x, 0.0f, curPos.z) + (new Vector3(Mathf.Cos(headingTheta + Mathf.PI), 0.0f, Mathf.Sin(headingTheta + Mathf.PI)) * ELLIPSE_Y_RADIUS);
        southBorder.transform.localScale = new Vector3(PE_SHORT_DIMENSION, 0.5f, 0.1f);
        southBorder.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        southBorder.AddComponent<BoxCollider>();
        southBorder.GetComponent<BoxCollider>().isTrigger = true;
        southBorder.AddComponent<PhysicalLocationTracking>();

        eastBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        eastBorder.transform.position = new Vector3(curPos.x, 0.0f, curPos.z) + (new Vector3(Mathf.Cos(headingTheta - (Mathf.PI/2.0f)), 0.0f, Mathf.Sin(headingTheta - (Mathf.PI/2.0f))) * ELLIPSE_X_RADIUS);
        eastBorder.transform.localScale = new Vector3(PE_LONG_DIMENSION, 0.5f, 0.1f);
        eastBorder.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        eastBorder.AddComponent<BoxCollider>();
        eastBorder.GetComponent<BoxCollider>().isTrigger = true;
        eastBorder.AddComponent<PhysicalLocationTracking>();

        westBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        westBorder.transform.position = new Vector3(curPos.x, 0.0f, curPos.z) + (new Vector3(Mathf.Cos(headingTheta + (Mathf.PI/2.0f)), 0.0f, Mathf.Sin(headingTheta + (Mathf.PI/2.0f))) * ELLIPSE_X_RADIUS);
        westBorder.transform.localScale = new Vector3(PE_LONG_DIMENSION, 0.5f, 0.1f);
        westBorder.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        westBorder.AddComponent<BoxCollider>();
        westBorder.GetComponent<BoxCollider>().isTrigger = true;
        westBorder.AddComponent<PhysicalLocationTracking>();

        northBorder.transform.LookAt(hmd.transform);
        southBorder.transform.LookAt(hmd.transform);
        eastBorder.transform.LookAt(hmd.transform);
        westBorder.transform.LookAt(hmd.transform);
        westBorder.name = "West Border";
        eastBorder.name = "East Border";
        northBorder.name = "North Border";
        southBorder.name = "South Border";
        Debug.Log("north boarder " + northBorder.transform.position);
        Debug.Log("south boarder " + southBorder.transform.position);
        Debug.Log("east boarder " + eastBorder.transform.position);
        Debug.Log("west boarder " + westBorder.transform.position);
        Debug.Log("=================================================");
        if (!drawPhysEnvMarkers){
        westBorder.GetComponent<Renderer>().enabled = false;
        eastBorder.GetComponent<Renderer>().enabled = false;
        northBorder.GetComponent<Renderer>().enabled = false;
        southBorder.GetComponent<Renderer>().enabled = false;
        }

        // boundaryMade = true;
        // envsCentered = true;
    }
}
