using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{   
    [field: Header("Camera")]
    public Transform cam;

    [field: Header("Movement Variables")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float maxSpeed = 6f;

    private float currentSpeed = 0f;
    public float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;
    private CharacterController controller;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        }
        else
        {
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0f);
        }


        if (direction.magnitude >= 0.1f)
        {
            // Movement direction relative to camera
            Vector3 moveDirection = cam.forward * direction.z + cam.right * direction.x;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            // Move only
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }


        // Always rotate player to face same direction as camera
        Vector3 camForward = cam.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // smooth
        }

    }

    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }
}