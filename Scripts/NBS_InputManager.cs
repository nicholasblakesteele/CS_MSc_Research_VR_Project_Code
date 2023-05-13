using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit; //.UI;

public class NBS_InputManager : MonoBehaviour
{
    // Editor objects
    public NBS_StudyManager studyManager; // Needed to access log queue
    public GameObject leftRayController; // Note: hardcoded to assume teleport is on left controller
    public GameObject rightRayController; // Note: hardcoded to assume UI interaction is on left controller

    // Convenience variables
    private XRRayInteractor leftRayInteractor;
    private XRRayInteractor rightRayInteractor;
    private NBS_LocomotionState previousLocomotionState; // For menu management only
    private bool allowMouseInputForTeleportation; // Will be set as to whether we are in the Unity editor mode or on device
    private NBS_LocomotionState currentLocomotionState;

    // Scripts on this object
    private NBS_Locomotion_ContinuousJoystick continuousJoystickManager; 
    private NBS_Locomotion_DirectTeleport directTeleportManager; 
    private NBS_Locomotion_Rotation rotationManager; 
    private NBS_Locomotion_StepTeleport stepTeleportManager;
    private NBS_Strings stringsManager;
        
    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        // Assumes the scripts for each locomotion method have been added to this object

        rotationManager = this.GetComponent<NBS_Locomotion_Rotation>();
        continuousJoystickManager = this.GetComponent<NBS_Locomotion_ContinuousJoystick>();
        directTeleportManager = this.GetComponent<NBS_Locomotion_DirectTeleport>();
        stepTeleportManager = this.GetComponent<NBS_Locomotion_StepTeleport>();

        leftRayInteractor = leftRayController.GetComponent<XRRayInteractor>();
        rightRayInteractor = rightRayController.GetComponent<XRRayInteractor>();

        stringsManager = this.GetComponent<NBS_Strings>();

        // Set if mouse is enabled or not. Update for other inputs
        // These are 2 separate variables, in the future, we can't assume editor = mouse input

        if (Application.isEditor == false)
        {
            allowMouseInputForTeleportation = false;
        }
        else
        {
            allowMouseInputForTeleportation = true;
        }
    }

    private void Start()
    {
        // Not used
    }

    private void Update()
    {
        // Override controller use if teleporting is in progress

        if (currentLocomotionState == NBS_LocomotionState.DirectTeleport_Instant || currentLocomotionState == NBS_LocomotionState.DirectTeleport_Dash) {
                    
            if (directTeleportManager.IsTeleporting() == true && Application.isEditor == false && allowMouseInputForTeleportation == false)
            {
                leftRayController.SetActive(false);
            }
            else if (directTeleportManager.IsTeleporting() == false && Application.isEditor == false && allowMouseInputForTeleportation == false)
            {
                leftRayController.SetActive(true);
            }            
        }
        else if (currentLocomotionState == NBS_LocomotionState.StepTeleport_Instant)
        {
            if (stepTeleportManager.IsTeleporting() == true && Application.isEditor == false && allowMouseInputForTeleportation == false)
            {
                leftRayController.SetActive(false);
            }
            else if (stepTeleportManager.IsTeleporting() == false && Application.isEditor == false && allowMouseInputForTeleportation == false)
            {
                leftRayController.SetActive(true);
            }
        }
        else
        {
            leftRayController.SetActive(false);
        }
    }
     
    // ************************
    // Control scheme methods
    // ************************

    public void UpdateLocomotionControllers()
    {
        currentLocomotionState = GetLocomotionState();

        if (currentLocomotionState == NBS_LocomotionState.Disabled)
        {
            leftRayController.SetActive(false);
            rightRayController.SetActive(false);
            rotationManager.SetNBSEnabled(true); // Enabled to prevent sickness caused by unexpected spinning during loading
            continuousJoystickManager.SetNBSEnabled(false);
            directTeleportManager.SetNBSEnabled(false);
            stepTeleportManager.SetNBSEnabled(false);
        }        
        else if (currentLocomotionState == NBS_LocomotionState.ContinuousJoystick)
        {
            leftRayController.SetActive(false);
            rightRayController.SetActive(false);
            rotationManager.SetNBSEnabled(true);
            continuousJoystickManager.SetNBSEnabled(true);
            directTeleportManager.SetNBSEnabled(false);
            stepTeleportManager.SetNBSEnabled(false);
        }
        else if (currentLocomotionState == NBS_LocomotionState.DirectTeleport_Instant || currentLocomotionState == NBS_LocomotionState.DirectTeleport_Dash)
        {
            leftRayController.SetActive(true); 
            rightRayController.SetActive(false);
            rotationManager.SetNBSEnabled(true);
            continuousJoystickManager.SetNBSEnabled(false);
            directTeleportManager.SetNBSEnabled(true);
            stepTeleportManager.SetNBSEnabled(false);            
        }
        else if (currentLocomotionState == NBS_LocomotionState.StepTeleport_Instant)
        {
            leftRayController.SetActive(true);
            rightRayController.SetActive(false);
            rotationManager.SetNBSEnabled(true);
            continuousJoystickManager.SetNBSEnabled(false);
            directTeleportManager.SetNBSEnabled(false);
            stepTeleportManager.SetNBSEnabled(true);            
        }
    }

    // ************************
    // Input handling methods
    // ************************

    private void OnPressButtonToBegin(InputValue value)
    {
        print("OnPressButtonToBegin:");

        if (studyManager.GetStudyPauseStatus() == true)
        {
            studyManager.ButtonPressedToBegin(); 
        }
        else
        {
            // Ignore button press - route is in progress
        }
    }

    // CONTINUOUS MOVEMENT METHOD
    private void OnContinuousMove(InputValue value)
    {
        // Gatekeep to ensure input type matches movement state

        if (GetLocomotionState() == NBS_LocomotionState.ContinuousJoystick)
        {
            Vector2 moveVector = value.Get<Vector2>();

            continuousJoystickManager.SetMoveVector(moveVector.x, 0, moveVector.y); // Player y locked to 0 - no height change needed
        }
    }

    // PLAYER LOOK ROTATION METHOD
    private void OnLookRotation(InputValue value)
    {
        // Gatekeep to ensure input type matches movement state

        // NOTE: below only works for PC tests and VR headset
        // NOTE: VR controller joysticks are disabled in Unity Input Manager so rotation only happens with headset
        // NOTE: Add VR controller joystick to Unity Input Manager to enable again
        
        if (GetLocomotionState() != NBS_LocomotionState.Disabled)
        {
            Vector2 lookVector = value.Get<Vector2>();

            rotationManager.RotatePlayer(lookVector.x); 
        }
    }

    // TELEPORTATION METHODS
    private void OnTeleportTriggered(InputValue value)
    {
        // NOTE: this fires/calculates even if the user is already teleporting or if the destination is NOT valid, so need to check for destination validity
        // NOTE: it is the reponsibility of the teleportion class to prevent actual teleportion

        // Gatekeep to ensure input type matches movement state

        NBS_LocomotionState currentLocomotionState = GetLocomotionState();

        if (currentLocomotionState == NBS_LocomotionState.DirectTeleport_Instant || currentLocomotionState == NBS_LocomotionState.DirectTeleport_Dash
            || currentLocomotionState == NBS_LocomotionState.StepTeleport_Instant)
        {
            // *** Handle controller teleport ***
            // Take the value from the method ONLY for controller teleport

            // NOTE: really important to check for isEditor and allowMouseInputForTeleportation, otherwise you can get TWO inputs firing which leads to unexpected positioning
            // These are 2 separate variables, in the future, we can't assume editor = mouse input

            if (leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit) && Application.isEditor == false && allowMouseInputForTeleportation == false)
            {
                // Get endpoint

                Vector3 desiredDestination = raycastHit.point; // the coordinate that the ray hits                    
                
                GameObject hitObject = raycastHit.collider.gameObject;

                ValidateRaycastBeforeTeleportation(hitObject, desiredDestination);
            }

            // *** Handle mouse input for teleport ***
            // For testing on computer only

            else if (Application.isEditor == true && allowMouseInputForTeleportation == true) {

                // *** Mouse position *** 
                // For the moment, if not a controller, assume it is a mouse position

                Plane plane = new Plane(Vector3.up, 0);
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());                
                float distance;
                
                // Get endpoint

                if (plane.Raycast(ray, out distance))
                {
                    Vector3 desiredDestination = ray.GetPoint(distance);
                   
                    // Get object hit at endpoint

                    if (Physics.Raycast(ray, out raycastHit))
                    {
                        GameObject hitObject = raycastHit.collider.gameObject;

                        ValidateRaycastBeforeTeleportation(hitObject, desiredDestination);
                    }

                    // Else do nothing, no valid surface found
                }                 
            }  // End of mouse input testing
        } 
    }

    private void ValidateRaycastBeforeTeleportation(GameObject hitObject, Vector3 desiredDestination)
    {
        // Note: this exists in this class as it is validates the input before allowing teleportation

        if (hitObject.layer == 3 || hitObject.tag == "Waypoint") // layer = 3 == Teleport
        {
            print("Valid teleport layer hit");

            AddToStudyManagerLogQueue(GetLogStringForKey("PlayerInput"), GetLogStringForKey("DidTeleport"));

            if (GetLocomotionState() == NBS_LocomotionState.DirectTeleport_Instant)
            {
                directTeleportManager.MovePlayer(desiredDestination, NBS_Locomotion_TeleportionMovementType.Instant);
            }
            else if (GetLocomotionState() == NBS_LocomotionState.DirectTeleport_Dash)
            {
                directTeleportManager.MovePlayer(desiredDestination, NBS_Locomotion_TeleportionMovementType.Dash);                
            }
            else if (GetLocomotionState() == NBS_LocomotionState.StepTeleport_Instant)
            {
                stepTeleportManager.MovePlayer(desiredDestination, NBS_Locomotion_TeleportionMovementType.Instant);              
            }           
        }
    }

    // ************************
    // Convenience methods
    // ************************

    private NBS_LocomotionState GetLocomotionState()
    {
        return studyManager.GetLocomotionState();
    }

    private void AddToStudyManagerLogQueue(string type, string value)
    {
        studyManager.AddToWriteQueue(type, value);
    }

    private string GetLogStringForKey(string key)
    {
        return stringsManager.GetLogStringForKey(key);
    }    
}