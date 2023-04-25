using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class RDW : MonoBehaviour
{
    public Transform headTransform;
    public Transform centerOfPlayArea;
    public float playAreaWidth = 4.0f;
    public float playAreaLength = 4.0f;
    public float minTranslationGain = 1.0f;
    public float maxTranslationGain = 1.5f;
    public float rotationGain = 1.1f;

    private Transform playerTransform;
    private CharacterController characterController;
    private Vector3 previousHeadOrientation;
    private Vector3 previousRealWorldPosition;

    private void Start()
    {
        playerTransform = GetComponent<Transform>();
        characterController = gameObject.AddComponent<CharacterController>();
        characterController.height = 2.0f;
        characterController.center = new Vector3(0, 1, 0);
        previousHeadOrientation = headTransform.forward;
        previousRealWorldPosition = GetPositionInPlayArea();
    }

    private void Update()
    {
        ApplyRotationGain();
        ApplyTranslationGain();
    }

    private void ApplyRotationGain()
    {
        Vector3 currentHeadOrientation = headTransform.forward;
        float angleDifference = Vector3.SignedAngle(previousHeadOrientation, currentHeadOrientation, Vector3.up);
        float rotationMultiplier = Mathf.Clamp(angleDifference, -rotationGain, rotationGain);

        playerTransform.RotateAround(headTransform.position, Vector3.up, rotationMultiplier);
        previousHeadOrientation = currentHeadOrientation;
    }

    private void ApplyTranslationGain()
    {
        Vector3 realWorldPosition = GetPositionInPlayArea();
        Vector3 realWorldDelta = realWorldPosition - previousRealWorldPosition;
        float dynamicTranslationGain = CalculateDynamicTranslationGain();
        Vector3 worldSpaceDirection = playerTransform.TransformDirection(realWorldDelta) * dynamicTranslationGain;

        characterController.SimpleMove(worldSpaceDirection);
        previousRealWorldPosition = realWorldPosition;
    }

    private float CalculateDynamicTranslationGain()
    {
        float distanceToCenter = Vector3.Distance(playerTransform.position, centerOfPlayArea.position);
        float maxDistance = Mathf.Sqrt(playAreaWidth * playAreaWidth + playAreaLength * playAreaLength) / 2;
        float dynamicGain = Mathf.Lerp(minTranslationGain, maxTranslationGain, distanceToCenter / maxDistance);
        return dynamicGain;
    }

    private Vector3 GetPositionInPlayArea()
    {
        Vector3[] boundaryCorners = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        Vector2 playAreaDimensions = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);

        float minX = Mathf.Min(boundaryCorners[0].x, boundaryCorners[2].x);
        float minZ = Mathf.Min(boundaryCorners[0].z, boundaryCorners[2].z);

        Vector3 playAreaCenter = new Vector3(minX + playAreaDimensions.x / 2, 0, minZ + playAreaDimensions.y / 2);
        Vector3 positionInPlayArea = headTransform.position - playAreaCenter;

        return positionInPlayArea;
    }
}