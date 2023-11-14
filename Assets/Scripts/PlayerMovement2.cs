using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    private InputManager inputManager;
    private Rigidbody rb;

    [SerializeField]
    private float walkSpeed = 4f;
    [SerializeField]
    private float sprintSpeed = 7f;

    private bool isGrounded;


    private void OnCollisonEnter(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisonExit(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        float forward = inputManager.inputMaster.Movement.Forward.ReadValue<float>();
        float right = inputManager.inputMaster.Movement.Right.ReadValue<float>();
        Vector3 moveDirection = transform.forward * forward + transform.right * right;

        moveDirection *= inputManager.inputMaster.Movement.Sprint.ReadValue<float>() == 0 ? walkSpeed : sprintSpeed;

        transform.localScale = new Vector3(1, inputManager.inputMaster.Movement.Crouch.ReadValue<float>() == 0 ? 1f : 0.72618f, 1);

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }
}
