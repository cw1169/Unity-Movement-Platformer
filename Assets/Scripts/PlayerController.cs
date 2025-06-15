using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public InputSystem_Actions InputSystem;

    [field: Header("Camera")]
    public Transform cam;

    [field: Header("Movement Variables")]
    public float acceleration = 200f;
    public float deceleration = 150f;
    public float maxSpeed = 20f;
    public float turnSmoothTime = 2f;
    public float gravityValue = -9.81f;
    public float jumpVelocity = 30f;


    private InputAction move, jump; //declare input actions
    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocity;
    private CharacterController controller;
    private bool isGrounded, jumping;


    private void Awake()
    {
        InputSystem = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        move = InputSystem.Player.Move;
        jump = InputSystem.Player.Jump;
        move.Enable();
        jump.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable(); 
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null)
            Debug.LogError("Camera Transform 'cam' is not assigned! Please assign it in Inspector.");
    }


    void Update()
    {
        Vector2 movementValues = move.ReadValue<Vector2>();
        float horizontal = movementValues.x;
        float vertical = movementValues.y;

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Camera relative directions (flattened)
        Vector3 camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 targetMoveDirection = camForward * inputDirection.z + camRight * inputDirection.x;
        targetMoveDirection.Normalize();

        if (inputDirection.magnitude >= 0.1f)
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetMoveDirection * maxSpeed, acceleration * Time.deltaTime);
        }

        else
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }


        // Rotate player to face camera forward smoothly
        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothTime * 10f * Time.deltaTime);
        }

        isGrounded = IsGrounded();
        //Jump Code
        if (isGrounded && verticalVelocity.y < 0) // Reset vertical velocity if grounded and moving downwards
        {
            verticalVelocity.y = -2f; // Small downward force to ensure it sticks to the ground
        }

        if (jump.triggered && isGrounded)
        {
            // Calculate jump velocity based on desired jump height and gravity
            // v = sqrt(2gh)
            verticalVelocity.y = Mathf.Sqrt(jumpVelocity * -2f * gravityValue);
        }
        if (!isGrounded)
        {
            // fall at quadratic rate
            verticalVelocity.y += gravityValue* - gravityValue * Time.deltaTime;
        }


        Vector3 finalVelocity = horizontalVelocity + verticalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);

    }


    // Work in progress
    public bool IsGrounded()
    {
        LayerMask layerMask = LayerMask.GetMask("Ground");
        float rayLength = controller.radius + 1.0f;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), rayLength, layerMask))

        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * rayLength, Color.green);
            Debug.Log("is grounded");
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * rayLength, Color.white);
            Debug.Log("not grounded");
            return false;
        }
    }

    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }
}
