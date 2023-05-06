using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class RedirectedWalking : MonoBehaviour
{
    private Transform headTransform;
    private float playAreaWidth;
    private float playAreaLength;
    private Vector3 playAreaCenter;

    public float minTranslationGain = 1.0f;
    public float maxTranslationGain = 1.5f;
    public GameObject ground;
    public GameObject wall;

    private Transform playerTransform;
    private Vector3 previousRealWorldPosition;
    private Vector3 groundHeight;
    private Vector3 wallHeight;
    private float playerY;

    private void Start()
    {
        headTransform = Camera.main.transform;
        playerTransform = GetComponent<Transform>();

        Vector3[] playAreaCorners = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        CalculatePlayAreaDimensions(playAreaCorners);
        playAreaCenter = CalculatePlayAreaCenter(playAreaCorners);

        previousRealWorldPosition = GetPositionInPlayArea();

        groundHeight = ground.GetComponent<Transform>().position;
        wallHeight = wall.GetComponent<Transform>().position;
        Debug.Log("ground height: " + groundHeight);
        Debug.Log("wall height: " + wallHeight);

    }

    private void Update()
    {
        ApplyTranslationGain();
        groundHeight = ground.GetComponent<Transform>().position;
        wallHeight = wall.GetComponent<Transform>().position;
        Debug.Log("ground height: " + groundHeight);
        Debug.Log("wall height: " + wallHeight);
    }

    private void ApplyTranslationGain()
    {
        Vector3 realWorldPosition = GetPositionInPlayArea();
        Debug.Log("realWorldPosition: " + realWorldPosition);
        Vector3 realWorldDelta = realWorldPosition - previousRealWorldPosition;
        float dynamicTranslationGain = CalculateDynamicTranslationGain();
        Vector3 worldSpaceDirection = playerTransform.TransformDirection(realWorldDelta) * dynamicTranslationGain;
        worldSpaceDirection.y = 0;
        // Clamp worldSpaceDirection to prevent large or infinite position values
        worldSpaceDirection = Vector3.ClampMagnitude(worldSpaceDirection, Mathf.Max(playAreaWidth, playAreaLength));
        Debug.Log("worldSpaceDirection: " + worldSpaceDirection);

        playerTransform.position += worldSpaceDirection;
        previousRealWorldPosition = realWorldPosition;
    }

    private float CalculateDynamicTranslationGain()
    {
        float distanceToCenter = Vector3.Distance(playerTransform.position, playAreaCenter);
        float maxDistance = Mathf.Sqrt(playAreaWidth * playAreaWidth + playAreaLength * playAreaLength) / 2;
        float dynamicGain = Mathf.Lerp(minTranslationGain, maxTranslationGain, distanceToCenter / maxDistance);
        return dynamicGain;
    }

    private Vector3 GetPositionInPlayArea()
    {
        Vector3 positionInPlayArea = headTransform.position - playAreaCenter;
        Debug.Log(positionInPlayArea);
        return positionInPlayArea;
    }

    private Vector3 CalculatePlayAreaCenter(Vector3[] playAreaCorners)
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 corner in playAreaCorners)
        {
            sum += corner;
        }
        return sum / playAreaCorners.Length;
    }

    private void CalculatePlayAreaDimensions(Vector3[] playAreaCorners)
    {
        Vector3 cornerA = playAreaCorners[0];
        Vector3 cornerB = playAreaCorners[1];
        Vector3 cornerC = playAreaCorners[2];

        playAreaWidth = Vector3.Distance(cornerA, cornerB);
        playAreaLength = Vector3.Distance(cornerB, cornerC);
    }
}
