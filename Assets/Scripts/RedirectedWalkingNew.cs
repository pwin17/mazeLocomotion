using UnityEngine;

public class RedirectedWalking : MonoBehaviour
{
    public Transform centerEyeAnchor;
    public Transform leftController;
    public Transform rightController;
    public float minTranslationGain = 1.0f;
    public float maxTranslationGain = 10.0f; // Increase the maximum translation gain value
    public float playAreaSize = 1.397f; // Adjust the play area size to match the physical space
    public float walkingThreshold = 0.3f;

    public float minRotationGain = 0.9f;
    public float maxRotationGain = 1.1f;

    private CharacterController characterController;
    private Vector3 previousHeadPosition;
    private Vector3 previousLeftHeadPosition;
    private Vector3 previousRightHeadPosition;
    public float movementSpeed = 12.0f;

    private float initialForwardRotation;

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
    }

    void Update()
    {
        Vector3 leftControllerDelta = leftController.position - previousLeftHeadPosition;
        Vector3 rightControllerDelta = rightController.position - previousRightHeadPosition;

        if (leftControllerDelta.magnitude > walkingThreshold || rightControllerDelta.magnitude > walkingThreshold)
        {
            Vector3 averageDirection = (leftControllerDelta + rightControllerDelta) / 2;
            Vector3 userFacingDirection = centerEyeAnchor.forward;
            userFacingDirection.y = 0;
            userFacingDirection.Normalize();

            Vector3 movementDirection = userFacingDirection * averageDirection.magnitude;
            transform.position += movementDirection * movementSpeed * CalculateDynamicTranslationGain(centerEyeAnchor.position);

            float currentForwardRotation = centerEyeAnchor.eulerAngles.y;
            float rotationDelta = Mathf.DeltaAngle(initialForwardRotation, currentForwardRotation);
            float rotationGain = CalculateDynamicRotationGain(centerEyeAnchor.position);

            transform.Rotate(0, rotationDelta * (rotationGain - 1), 0);

            initialForwardRotation = currentForwardRotation;
        }

        previousLeftHeadPosition = leftController.position;
        previousRightHeadPosition = rightController.position;
    }

    private float CalculateDynamicTranslationGain(Vector3 headPosition)
    {
        Vector3 playAreaCenter = new Vector3(playAreaSize / 2.0f, headPosition.y, playAreaSize / 2.0f);
        float distanceToCenter = Vector3.Distance(headPosition, playAreaCenter);
        float maxDistance = playAreaSize / 2.0f;
        float dynamicGain = Mathf.Lerp(maxTranslationGain, minTranslationGain, distanceToCenter / maxDistance);
        return dynamicGain;
    }

    private float CalculateDynamicRotationGain(Vector3 headPosition)
    {
        Vector3 playAreaCenter = new Vector3(playAreaSize / 2.0f, headPosition.y, playAreaSize / 2.0f);
        

        float distanceToCenter = Vector3.Distance(headPosition, playAreaCenter);
        float maxDistance = playAreaSize / 2.0f;
        float dynamicGain = Mathf.Lerp(minRotationGain, maxRotationGain, distanceToCenter / maxDistance);
        return dynamicGain;
    }
}
