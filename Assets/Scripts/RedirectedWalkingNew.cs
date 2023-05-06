using UnityEngine;

public class RedirectedWalkingNew : MonoBehaviour
{
    public Transform centerEyeAnchor;
    public Transform leftController;
    public Transform rightController;
    public float minTranslationGain;// = 1.0f;
    public float maxTranslationGain = 10.0f; // Increase the maximum translation gain value
    public float playAreaSize = 1.397f; // Adjust the play area size to match the physical space
    static float PE_SHORT_DIMENSION = 4.3f;
    static float PE_LONG_DIMENSION = 6.125F;
    public float walkingThreshold = 0.3f;

    public float minRotationGain = 0.9f;
    public float maxRotationGain = 1.1f;

    private CharacterController characterController;
    private Vector3 previousHeadPosition;
    private Vector3 previousLeftHeadPosition;
    private Vector3 previousRightHeadPosition;
    public float movementSpeed = 12.0f;
    public float TranslationGain = 5.0f;
    private float initialForwardRotation;
    GameObject resetParentObj;
    GameObject VE;
    bool isResetting = false;

    void Start()
    {
        if (leftController != null)
        {
            previousLeftHeadPosition = leftController.position;
        }
        if (rightController != null)
        {
            previousRightHeadPosition = rightController.position;
        }
        if (centerEyeAnchor != null)
        {
            previousHeadPosition = centerEyeAnchor.position;
            initialForwardRotation = centerEyeAnchor.eulerAngles.y;
        }
        resetParentObj = new GameObject("Reset Parent Obj"); 
        VE = GameObject.Find("VE");
        
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            isResetting = !isResetting;
            if (isResetting)
            {
                resetParentObj.transform.position = centerEyeAnchor.position;
                resetParentObj.transform.forward = new Vector3(centerEyeAnchor.forward.x, 0.0f, centerEyeAnchor.forward.z);
                VE.transform.parent = resetParentObj.transform;
            }
            else
            {
                VE.transform.parent = null;
            }
            
        }
        if (isResetting)
        {
            resetParentObj.transform.forward = new Vector3(centerEyeAnchor.forward.x, 0.0f, centerEyeAnchor.forward.z);
            resetParentObj.transform.position = centerEyeAnchor.position;
        }
        else
        {
            Vector3 leftControllerDelta = leftController.position - previousLeftHeadPosition;
            Vector3 rightControllerDelta = rightController.position - previousRightHeadPosition;
            Debug.Log("left :" + leftControllerDelta.magnitude);
            Debug.Log("right :" + rightControllerDelta.magnitude);

            if (leftControllerDelta.magnitude > walkingThreshold || rightControllerDelta.magnitude > walkingThreshold)
            {
                ApplyTranslationGain(leftControllerDelta, rightControllerDelta);
                ApplyRotationGain();
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
        Vector3 playAreaCenter = new Vector3(playAreaSize / 2.0f, headPosition.y, playAreaSize / 2.0f);
        
        float distanceToCenter = Vector3.Distance(headPosition, playAreaCenter);
        float maxDistance = playAreaSize / 2.0f;
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
        transform.position += movementDirection * movementSpeed * TranslationGain * Time.deltaTime;
    }

    private void ApplyRotationGain()
    {
        float currentForwardRotation = centerEyeAnchor.eulerAngles.y;
        float rotationDelta = Mathf.DeltaAngle(initialForwardRotation, currentForwardRotation);
        float rotationGain = CalculateDynamicRotationGain(centerEyeAnchor.position);

        //transform.Rotate(0, rotationDelta * (rotationGain - 1) * Time.deltaTime, 0);

        initialForwardRotation = currentForwardRotation;
    }
}
