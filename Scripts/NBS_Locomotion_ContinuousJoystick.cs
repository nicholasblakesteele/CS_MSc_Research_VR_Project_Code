using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBS_Locomotion_ContinuousJoystick : MonoBehaviour
{
    public Camera mainCamera; // For walking in direction user is looking
    private Rigidbody rigidBody; // Needed for object collisions
    private Vector3 inputVector;

    // State variables
    private bool isEnabled = false;
    private float moveSpeed = 1.4f; // 1 is a nice speed. 1.5 is quite fast
    
    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {     
        rigidBody = this.GetComponent<Rigidbody>();
        inputVector = new Vector3(0, 0, 0);
    }

    private void Start()
    {
        // Not used
    }

    private void Update()
    {
        // Not used
    }

    private void FixedUpdate()
    {
        MovePlayerOnFrame(); // must happen in physics frame update in order for collisions to happen
    }

    // ************************
    // Movement methods
    // ************************

    public void SetMoveVector(float x, float y, float z)
    {
        inputVector = new Vector3(x, y, z) * moveSpeed;
    }

    private void MovePlayerOnFrame()
    {
        if (isEnabled == true)
        {
            // In order for movement to be relative to where the user is facing, first get the normalised forward direction of the player's camera (not camera)        
            Vector3 cameraFacingVector = mainCamera.transform.rotation * Vector3.forward;
            cameraFacingVector = new Vector3(cameraFacingVector.x, 0f, cameraFacingVector.z).normalized;

            // Now multiply the inputVector by the player's facing vector so that forward movement is in the direction of the player's forward and not the overall world
            Vector3 moveVector = Quaternion.FromToRotation(Vector3.forward, cameraFacingVector) * inputVector;

            rigidBody.MovePosition(rigidBody.position + moveVector * Time.deltaTime);
        }
        else
        {
            inputVector = new Vector3(0, 0, 0);
        }
    }

    // ************************
    // Accessor methods
    // ************************

    public void SetNBSEnabled(bool _isEnabled)
    {
        isEnabled = _isEnabled;
    }
}