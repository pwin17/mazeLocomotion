using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    private void onTriggerEnter(Collider other)
    {
        IsHitting.isWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("----------------Trigger Staying-------------");
        IsHitting.isWall = false;
    }
}
