using UnityEngine;

public class RedirectedWalkingNew : MonoBehaviour
{
    // Trackers
    public Transform centerEyeAnchor;
    public float minRotationGain = 0.9f;
    public float maxRotationGain = 1.1f;
    // Physical Boundary Size
    static float PE_SHORT_DIMENSION = 4.3f;
    static float PE_LONG_DIMENSION = 6.125F;
    float ELLIPSE_Y_RADIUS = PE_LONG_DIMENSION / 2.0f;
    float ELLIPSE_X_RADIUS = PE_SHORT_DIMENSION / 2.0f;
    public float walkingThreshold = 0.3f;
    private Vector3 previousHeadPosition;
    public float ConstantTranslationGain = 5.0f;
    private float initialForwardRotation;
    GameObject resetParentObj, VE;
    GameObject northBorder, eastBorder, westBorder, southBorder;
    bool drawPhysEnvMarkers = true;
    bool isResetting = false;

    void Start()
    {
        // Vector3 startPosition = centerEyeAnchor.position;
        initialForwardRotation = centerEyeAnchor.eulerAngles.y;
        
        resetParentObj = new GameObject("Reset Parent Obj"); 
        VE = GameObject.Find("VE");
        // CenterEnv();
    }

    void LateUpdate()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
            isResetting = !isResetting;
        if (IsHitting.isBoundary)
        {
            Debug.Log("IS hitting boundary");
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
            Vector3 centerEyeAnchorDelta = centerEyeAnchor.position - previousHeadPosition;
            
            if (Mathf.Abs(centerEyeAnchorDelta.z) > walkingThreshold)
            {
                Debug.Log("---------------");
                Debug.Log("Translation Gain Z happens here");
                Vector3 currOffset = ApplyTranslationGain(Vector3.forward);
                // RecenterBoundary(currOffset);
                Debug.Log("---------------");
                // ApplyRotationGain();
            }
            else if (Mathf.Abs(centerEyeAnchorDelta.x) > walkingThreshold)
            {
                Debug.Log("---------------");
                Debug.Log("Translation Gain X happens here");
                Vector3 currOffset = ApplyTranslationGain(Vector3.right);
                Debug.Log("---------------");
            }
        }
        previousHeadPosition = centerEyeAnchor.position;
        // CenterEnv();
    }
    private Vector3 ApplyTranslationGain(Vector3 appliedDirection)
    {
        Vector3 userFacingDirection = centerEyeAnchor.forward;
        userFacingDirection.Normalize();
        userFacingDirection = Vector3.Scale(userFacingDirection, appliedDirection); // only take the direction user is moving in
        Debug.Log("User Facing Direction" + userFacingDirection);

        Vector3 movementDirection = userFacingDirection;
        float translationGain = IsHitting.isWall ? 0.0f: ConstantTranslationGain;
        movementDirection *= translationGain;
        Vector3 newPosition = movementDirection;
        Vector3 prevPosition =transform.position;
        transform.position += newPosition * Time.deltaTime;
        Vector3 positionOffset = transform.position - prevPosition;
        return positionOffset;
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
        Vector3 curForward = centerEyeAnchor.forward;
        float headingTheta = Mathf.Atan2(curForward.z, curForward.x);
        float offsetToNorth = Vector2.SignedAngle(new Vector2(curForward.x, curForward.z), new Vector2(0.0f, 1.0f));
        float newAngle = headingTheta - (Mathf.PI/2.0f);

        // // Align the VE
        // VE.transform.position -= playerOriginOffset;
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
    void RecenterBoundary(Vector3 offset)
    {
        Debug.Log("Recentering happens here");
        eastBorder.transform.position += offset;
        westBorder.transform.position += offset;
        northBorder.transform.position += offset;
        southBorder.transform.position += offset;
    }
}
