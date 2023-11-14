using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float sprintSpeed;

    [SerializeField]
    private float groundDrag;

    [Header("Keybinds")]
    private KeyCode sprintKey = KeyCode.LeftShift;
    private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Crouching")]
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float crouchYScale;
    // [SerializeField]
    private float startYScale;

    [Header("Ground Check")]
    [SerializeField]
    private float playerHeight;

    [SerializeField]
    private LayerMask groundMask;

    bool isGrounded;

    [SerializeField]
    private Transform orientation;

    float horizontalInput;
    float verticalInput;

    float stamina;

    Vector3 moveDirection;

    Rigidbody rb;

    Slider slider;

    private MovementState state;
    private enum MovementState
    {
        Walking,
        Sprinting,
        Crouching
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        slider = FindObjectOfType<Slider>();

        startYScale = transform.localScale.y;

        stamina = 100f;

    }

    private void Update()
    {
        // ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.2f, groundMask);
        MyInput();
        SpeedControl();
        StateHandler();

        slider.value = stamina / 100f;

        // handle drag
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // start crouching

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        // check if player can stand up
        // if (Physics.Raycast(transform.position, Vector3.up, playerHeight / 2 + 0.2f, groundMask))

    }

    private void StateHandler()
    {
    

        if (isGrounded && Input.GetKey(sprintKey) && stamina > 0f)
        {
            if (IsInvoking(nameof(StartToRecover))) {
                CancelInvoke(nameof(StartToRecover));
            }   
            state = MovementState.Sprinting;
            moveSpeed = sprintSpeed;
            if (verticalInput != 0f || horizontalInput != 0f) {
                stamina -= 0.1f;
            } else if (verticalInput != 0f && horizontalInput != 0f) {
                stamina -= 0.2f;
            } else {
                stamina += 0.05f;
            }

            if (stamina < 0f)
                stamina = 0f;

        }
        else if (isGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            state = MovementState.Crouching;

            Invoke(nameof(StartToRecover), 0.1f);

            moveSpeed = crouchSpeed;
        }
        else
        {
            state = MovementState.Walking;
            moveSpeed = walkSpeed;
            
            if (stamina <= 0f) {
                Invoke(nameof(StartToRecover), 2.5f);
            } else if (stamina <= 100f) {
                Invoke(nameof(StartToRecover), 0.6f);
            }

        }
    }

    private void StartToRecover()
    {
        stamina += 0.05f;
        stamina = Mathf.Round(stamina * 100f) / 100f;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * /*Time.deltaTime*/ 10f, ForceMode.Acceleration);
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //  limit speed if needed
        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }
}
