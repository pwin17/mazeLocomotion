using UnityEngine;

public class RedirectedWalkingTranslationGain : MonoBehaviour
{
    public Transform centerEyeAnchor;
    public Transform leftController;
    public Transform rightController;
    public float minTranslationGain = 1.0f;
    public float maxTranslationGain = 3.0f;
    public float playAreaSize = 3.0f;
    public float walkingThreshold = 0.5f;

    private CharacterController characterController;
    private Vector3 previousHeadPosition;
    private Vector3 previousLeftHeadPosition;
    private Vector3 previousRightHeadPosition;
    public float movementSpeed = 10.0f;

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
        }
    }

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
        else
        {
            Vector3 leftControllerDelta = leftController.position - previousLeftHeadPosition;
            Vector3 rightControllerDelta = rightController.position - previousRightHeadPosition;
            if (leftControllerDelta.magnitude > walkingThreshold || rightControllerDelta.magnitude > walkingThreshold)
            {
                Debug.Log("left :" + leftControllerDelta.magnitude);
                Debug.Log("right :" + rightControllerDelta.magnitude);
                Vector3 averageDirection = (leftControllerDelta + rightControllerDelta) / 2;
                Vector3 userFacingDirection = centerEyeAnchor.forward;
                Debug.Log(userFacingDirection);
                userFacingDirection.y = 0;
                userFacingDirection.x = 0;
                userFacingDirection.Normalize();

                Vector3 movementDirection = userFacingDirection * averageDirection.magnitude;
                Debug.Log("movementDirection :" + movementDirection);
                Debug.Log("transform.position :" + transform.position);
                transform.position += movementDirection * movementSpeed;
            }

            previousLeftHeadPosition = leftController.position;
            previousRightHeadPosition = rightController.position;
        }
    }

    private float CalculateDynamicTranslationGain(Vector3 headPosition)
    {
        Vector3 playAreaCenter = new Vector3(playAreaSize / 2.0f, headPosition.y, playAreaSize / 2.0f);
        float distanceToCenter = Vector3.Distance(headPosition, playAreaCenter);
        float maxDistance = playAreaSize / 2.0f;
        float dynamicGain = Mathf.Lerp(maxTranslationGain, minTranslationGain, distanceToCenter / maxDistance);
        return dynamicGain;
    }
}
