using UnityEngine;

public class RedirectedWalkingNew : MonoBehaviour
{
    // Trackers
    public Transform centerEyeAnchor, leftController, rightController;
    // RDW Gains
    public float minTranslationGain;// = 1.0f;
    public float maxTranslationGain = 10.0f; // Increase the maximum translation gain value
    public float minRotationGain = 0.9f;
    public float maxRotationGain = 1.1f;
    // Physical Boundary Size
    static float PE_SHORT_DIMENSION = 4.3f;
    static float PE_LONG_DIMENSION = 6.125F;
    float ELLIPSE_Y_RADIUS = PE_LONG_DIMENSION / 2.0f;
    float ELLIPSE_X_RADIUS = PE_SHORT_DIMENSION / 2.0f;
    public float walkingThreshold = 0.3f;
    private Vector3 previousHeadPosition, previousLeftHeadPosition, previousRightHeadPosition;
    public float movementSpeed = 12.0f;
    public float ConstantTranslationGain = 5.0f;
    private float initialForwardRotation;
    GameObject resetParentObj, VE;
    bool isResetting = false;
    GameObject northBorder, eastBorder, westBorder, southBorder;
    bool drawPhysEnvMarkers = true;

    void Start()
    {
        previousLeftHeadPosition = leftController.position;
        previousRightHeadPosition = rightController.position;
        previousHeadPosition = centerEyeAnchor.position;
        initialForwardRotation = centerEyeAnchor.eulerAngles.y;
        
        resetParentObj = new GameObject("Reset Parent Obj"); 
        VE = GameObject.Find("VE");
        CenterEnv();
    }

    void LateUpdate()
    {
        if (IsHitting.isBoundary)
        {
            resetParentObj.transform.position = centerEyeAnchor.position;
            resetParentObj.transform.forward = new Vector3(centerEyeAnchor.forward.x, 0.0f, centerEyeAnchor.forward.z);
            VE.transform.parent = resetParentObj.transform;
        }
        else
        {
            VE.transform.parent = null;
            Vector3 leftControllerDelta = leftController.position - previousLeftHeadPosition;
            Vector3 rightControllerDelta = rightController.position - previousRightHeadPosition;

            if (leftControllerDelta.magnitude > walkingThreshold || rightControllerDelta.magnitude > walkingThreshold)
            {
                ApplyTranslationGain(leftControllerDelta, rightControllerDelta);
                // ApplyRotationGain();
            }

            previousLeftHeadPosition = leftController.position;
            previousRightHeadPosition = rightController.position;
        }
    }

    private float CalculateDynamicTranslationGain(Vector3 headPosition)
    {
        // Vector3 playAreaCenter = new Vector3(PE_LONG_DIMENSION / 2.0f, headPosition.y, PE_SHORT_DIMENSION / 2.0f);
        float maxDistance;
        Vector3 playAreaCenter = new Vector3(0.0f, 0.0f, 0.0f);
        float distanceToCenter = Vector3.Distance(headPosition, playAreaCenter);
        Debug.Log(distanceToCenter);
        Vector3 userFacingDirection = centerEyeAnchor.forward;
        if (Mathf.Abs(userFacingDirection.x) > Mathf.Abs(userFacingDirection.z)) // user is heading along long axis
        {
            maxDistance = PE_LONG_DIMENSION / 2.0f;
        }
        else // user is heading along short axis
        {
            maxDistance = PE_SHORT_DIMENSION / 2.0f;
        }
        float dynamicTranslationGain = Mathf.Lerp(maxTranslationGain, minTranslationGain, distanceToCenter / maxDistance);
        // Debug.Log("Dynamic Translation Gain: " + dynamicTranslationGain);
        return dynamicTranslationGain;
    }

    private float CalculateDynamicRotationGain(Vector3 headPosition)
    {
        float maxDistance;
        Vector3 playAreaCenter = new Vector3(0.0f, 0.0f, 0.0f); // new Vector3(playAreaSize / 2.0f, headPosition.y, playAreaSize / 2.0f);
        
        float distanceToCenter = Vector3.Distance(headPosition, playAreaCenter);
        // float maxDistance = playAreaSize / 2.0f;
        Vector3 userFacingDirection = centerEyeAnchor.forward;
        if (Mathf.Abs(userFacingDirection.x) > Mathf.Abs(userFacingDirection.z)) // user is heading along long axis
        {
            maxDistance = PE_LONG_DIMENSION / 2.0f;
        }
        else // user is heading along short axis
        {
            maxDistance = PE_SHORT_DIMENSION / 2.0f;
        }
        float dynamicRotationGain = Mathf.Lerp(minRotationGain, maxRotationGain, distanceToCenter / maxDistance);
        return dynamicRotationGain;
    }


    private void ApplyTranslationGain(Vector3 leftControllerDelta, Vector3 rightControllerDelta)
    {
        Vector3 averageDirection = (leftControllerDelta + rightControllerDelta) / 2;
        Vector3 userFacingDirection = centerEyeAnchor.forward;
        userFacingDirection.y = 0;
        userFacingDirection.x = 0;
        userFacingDirection.Normalize();

        Vector3 movementDirection = userFacingDirection;// * averageDirection.magnitude;
        // Debug.Log("transform position: " + transform.position);
        // transform.position += movementDirection * movementSpeed * CalculateDynamicTranslationGain(centerEyeAnchor.position) * Time.deltaTime;
        transform.position += movementDirection * movementSpeed * ConstantTranslationGain * Time.deltaTime;
    }

    private void ApplyRotationGain()
    {
        float currentForwardRotation = centerEyeAnchor.eulerAngles.y;
        float rotationDelta = Mathf.DeltaAngle(initialForwardRotation, currentForwardRotation);
        float rotationGain = CalculateDynamicRotationGain(centerEyeAnchor.position);

        //transform.Rotate(0, rotationDelta * (rotationGain - 1) * Time.deltaTime, 0);

        initialForwardRotation = currentForwardRotation;
    }
    void CenterEnv()
    {
        Vector3 curPos = centerEyeAnchor.position;
        Debug.Log("=================================================");
        Debug.Log("current position " + curPos);
        Vector3 curForward = centerEyeAnchor.forward;
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

        northBorder.transform.LookAt(centerEyeAnchor);
        southBorder.transform.LookAt(centerEyeAnchor);
        eastBorder.transform.LookAt(centerEyeAnchor);
        westBorder.transform.LookAt(centerEyeAnchor);
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
