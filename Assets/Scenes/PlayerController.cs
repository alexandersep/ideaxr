using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;

    // Movement parameters
    public float walkSpeed = 100.0f;
    public float rotateSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedZ = walkSpeed * Input.GetAxis("Vertical");
        float curSpeedX = walkSpeed * Input.GetAxis("Horizontal");

        Vector3 moveDirection = forward * curSpeedZ + right * curSpeedX;
        characterController.Move(moveDirection * Time.deltaTime);

        if (Input.GetMouseButton(0))
        {
            float angleX = -Input.GetAxis("Mouse Y") * rotateSpeed;
            float angleY = Input.GetAxis("Mouse X") * rotateSpeed;
            transform.rotation *= Quaternion.AngleAxis(angleX, new Vector3(1, 0, 0)) *
                                  Quaternion.AngleAxis(angleY, new Vector3(0, 1, 0));
        }
    }
}
