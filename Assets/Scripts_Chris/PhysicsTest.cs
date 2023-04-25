using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PhysicsTest : MonoBehaviour
{
    public float speed = 5.0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        movement = transform.TransformDirection(movement);
        movement *= speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }
}

