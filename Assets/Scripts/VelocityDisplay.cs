using UnityEngine;

public class VelocityDisplay : MonoBehaviour
{
    public PlayerController playerController; // assign in Inspector
    public Vector3 velocity;

    void Update()
    {  
        velocity = playerController.GetVelocity();
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(10, 10, 300, 20), "Velocity: " + velocity.ToString("F2"));
    }
}