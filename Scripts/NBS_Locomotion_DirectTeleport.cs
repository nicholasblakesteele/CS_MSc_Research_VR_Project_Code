using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBS_Locomotion_DirectTeleport : MonoBehaviour
{
    // Editor variable
    public AnimationCurve animationCurve;

    // Convenience variables
    private NBS_PlayerManager playerManager;

    // State variables
    private bool isEnabled = false;
    private Rigidbody rigidBody; // Needed for object detection
    private bool isTeleporting = false;

    // Dash variables
    private bool enableDash = false;
    private Vector3 dashDestinationPosition;
    private Vector3 dashStartPosition;
    private float animationCurveFrameTime;

    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        playerManager = this.GetComponent<NBS_PlayerManager>();
        rigidBody = this.GetComponent<Rigidbody>();
        animationCurveFrameTime = 0; // Reset
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Not used
    }

    // Update is called once per frame
    private void Update()
    {
        // Dash movement 

        if (enableDash == true)
        {
            // Not a physics movement so does not need to happen in FrameUpdate()

            if (rigidBody.position == dashDestinationPosition)
            {
                print("Dash reached target, stop");

                enableDash = false;
                isTeleporting = false;
                animationCurveFrameTime = 0; // Reset

            } else
            {
                // Not yet reached destination, so keep following animationCurve (current set to EaseIn [start fast, slow on arrival])
                animationCurveFrameTime += Time.deltaTime;
                rigidBody.position = Vector3.Lerp(dashStartPosition, dashDestinationPosition, animationCurve.Evaluate(animationCurveFrameTime));
            }
        }
    }

    // ************************
    // Movement methods
    // ************************

    public void MovePlayer(Vector3 destinationPosition, NBS_Locomotion_TeleportionMovementType teleportationMovementType)
    {
        if (isEnabled == true && isTeleporting == false)
        {
            isTeleporting = true;

            if (teleportationMovementType == NBS_Locomotion_TeleportionMovementType.Instant)
            {
                print("Starting instant teleport");

                playerManager.TriggerScreenFade();

                MovePlayerInstantly(destinationPosition);

                isTeleporting = false;
            }
            else if (teleportationMovementType == NBS_Locomotion_TeleportionMovementType.Dash)
            {
                // NOTE: no fade

                print("Starting dash teleport");

                MovePlayerDash(destinationPosition);

                // Handle isTeleporting in update() not here, as only update() knows when destination is reached
            }
        }

        if (isTeleporting == true)
        {
            print("Direct teleport request ignored to " + destinationPosition + " : currently teleporting");
        }
    }
    
    private void MovePlayerInstantly(Vector3 destinationPosition)
    {
        rigidBody.position = destinationPosition;
    }

    private void MovePlayerDash(Vector3 destinationPosition)
    {        
        print("MovePlayerDash");

        dashStartPosition = rigidBody.position;
        dashDestinationPosition = destinationPosition;
        animationCurveFrameTime = 0; // Reset
        enableDash = true;

        // Update() will pick up these parameters and move as needed
    }

    // ************************
    // Accessor methods
    // ************************

    public bool IsTeleporting()
    {
        return isTeleporting;
    }

    public void SetNBSEnabled(bool _isEnabled)
    {
        isEnabled = _isEnabled;
    }
}