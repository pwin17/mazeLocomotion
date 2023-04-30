using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fly : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10.0f; // Adjust this value to change the movement speed

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardDirection = Camera.main.transform.forward;
        forwardDirection.y = 0f;
        forwardDirection.Normalize();

        // Get the direction of the left-hand thumbstick
        Vector2 thumbstickDirection = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // Calculate the movement direction based on both vectors
        Vector3 movementDirection = forwardDirection * thumbstickDirection.y + Camera.main.transform.right * thumbstickDirection.x;
        movementDirection.Normalize();

        float Speed = thumbstickDirection.magnitude * speed;
        // Move the camera in the calculated direction
        transform.position += movementDirection * speed * Time.deltaTime;
    }

}
