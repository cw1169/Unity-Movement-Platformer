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
    public float gravityValue = -30f;

    private InputAction move, jump; //declare input actions
    private Vector3 horizontalVelocity;
    private float jumpVelocity = 0f;
    private CharacterController controller;
    private bool isGrounded;


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

        Vector3 finalVelocity = horizontalVelocity + Vector3.up * jumpVelocity;
        controller.Move(finalVelocity * Time.deltaTime);

        // Rotate player to face camera forward smoothly
        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothTime * 10f * Time.deltaTime);
        }

        //Jump Code
        isGrounded = IsGrounded();
        if (jump.triggered && isGrounded)
        {           
            jumpVelocity = 10f;
        }
        else
        {
            jumpVelocity += gravityValue * Time.deltaTime;
        }
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
