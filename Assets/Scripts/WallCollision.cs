using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    private float timer = 0.0f;

    private void onTriggerEnter(Collider other)
    {
        IsHitting.isWall = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("----------------Trigger Staying-------------");
        if (IsHitting.isWall)
        {
            timer += Time.fixedDeltaTime;
        }
        if (timer > 5.0f) // hit for more than 5 seconds
        {
            timer = 0.0f;
            IsHitting.isWall = false;
        }
    }
}
