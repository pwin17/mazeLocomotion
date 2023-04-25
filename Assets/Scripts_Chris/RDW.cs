using UnityEngine;

public class RedirectedWalkingTranslationGain : MonoBehaviour
{
    public Transform centerEyeAnchor;
    public float minTranslationGain = 1.0f;
    public float maxTranslationGain = 3.0f;
    public float playAreaSize = 3.0f;

    private CharacterController characterController;
    private Vector3 previousHeadPosition;

    void Start()
    {
        characterController = gameObject.AddComponent<CharacterController>();
        characterController.height = 2.0f;
        characterController.center = new Vector3(0, 1, 0);

        if (centerEyeAnchor != null)
        {
            previousHeadPosition = centerEyeAnchor.position;
        }
    }

    void Update()
    {
        if (centerEyeAnchor != null)
        {
            Vector3 currentHeadPosition = centerEyeAnchor.position;
            Vector3 headPositionDelta = currentHeadPosition - previousHeadPosition;

            float dynamicTranslationGain = CalculateDynamicTranslationGain(currentHeadPosition);
            Vector3 worldSpaceDirection = transform.TransformDirection(headPositionDelta) * dynamicTranslationGain;

            characterController.Move(worldSpaceDirection);
            previousHeadPosition = currentHeadPosition;
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
