using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    [field: Header("Camera")]
    public Transform cam;

    [field: Header("Movement Variables")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float maxSpeed = 6f;

    private Vector3 horizontalVelocity;
    private float verticalVelocity = 0f;

    public float turnSmoothTime = 0.1f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null)
            Debug.LogError("Camera Transform 'cam' is not assigned! Please assign it in Inspector.");
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

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
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetMoveDirection * maxSpeed, acceleration * Time.deltaTime);
        else
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, deceleration * Time.deltaTime);

        // Gravity
        if (controller.isGrounded)
            verticalVelocity = -2f; // Small downward force to stick to ground
        else
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 finalVelocity = horizontalVelocity + Vector3.up * verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);

        // Rotate player to face camera forward smoothly
        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothTime * 10f * Time.deltaTime);
        }
    }

    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }
}
