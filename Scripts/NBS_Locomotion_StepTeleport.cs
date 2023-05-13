using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBS_Locomotion_StepTeleport : MonoBehaviour
{
    public GameObject midStepReticle;
    public GameObject endStepReticle;

    private NBS_PlayerManager playerManager;
    private Rigidbody rigidBody; // Needed for object collisions    
    private List<Vector3> stepPositionList;
    private List<GameObject> stepObjectsList;

    // State variables
    private bool isEnabled = false;
    private bool isTeleporting = false;
    
    private float stepDistance = 1.5f;
    
    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        playerManager = this.GetComponent<NBS_PlayerManager>(); 
        rigidBody = this.GetComponent<Rigidbody>();
        stepPositionList = new List<Vector3>();
        stepObjectsList = new List<GameObject>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Not used
    }

    // Update is called once per frame
    private void Update()
    {
        // Not used
    }

    // ************************
    // Movement methods
    // ************************

    public void MovePlayer(Vector3 destinationPosition, NBS_Locomotion_TeleportionMovementType teleportationMovementType)
    {
        if (isEnabled == true && isTeleporting == false)
        {
            print("Starting step teleport to " + destinationPosition);

            isTeleporting = true; 

            CalculateTeleportStepsToDestination(destinationPosition);

            DisplaySteps(destinationPosition); 

            StartCoroutine(MovePlayerAlongSteps(destinationPosition, teleportationMovementType));

            // WARNING! Do not place anything here otherwise it will run instantly after MovePlayerAlongSteps (unless this is desired)
        }

        if (isTeleporting == true)
        {
            print("Step teleport request ignored to " + destinationPosition + " : currently teleporting");
        }
    }
    
    private IEnumerator MovePlayerAlongSteps(Vector3 destinationPosition, NBS_Locomotion_TeleportionMovementType teleportationMovementType)
    {
        //int stepCount = 1;

        foreach (Vector3 stepPosition in stepPositionList)
        {
            //print("Step count to destination:" + stepCount); 
            print("Moving to next step: " + stepPosition);
            //nextStep = step;

            // Move according to teleportation movement type

            playerManager.TriggerScreenFade();            

            if (teleportationMovementType == NBS_Locomotion_TeleportionMovementType.Instant)
            {
                MovePlayerInstantly(stepPosition); // First step will happen instantly

            }

            // Pause temporarily before moving onto next step

            float stepDelayInSeconds = 1.0f; 

            if (stepPosition == destinationPosition)
            {
                // Do not pause if end reached
                //print("Final destination reached reached");
                stepDelayInSeconds = 0; 
            }

            yield return new WaitForSeconds(stepDelayInSeconds);

            //stepCount += 1; 
        }

        print("Finished step teleportation to destination");

        RemoveAllSteps();

        isTeleporting = false; 
    }

    private void MovePlayerInstantly(Vector3 step)
    {
        rigidBody.position = step;
    }

    // *********************************
    // Calculation and display of steps
    // *********************************

    private void CalculateTeleportStepsToDestination(Vector3 destinationPosition)
    {        
        // Begin calculation to destination
       
        Vector3 dummyPosition = this.transform.position;

        while (dummyPosition != destinationPosition)
        {
            // NOTE: must use dummy position not player position during step calculations
            // We are not moving the player yet

            float remainingDistanceToTravel = Vector3.Distance(dummyPosition, destinationPosition);
            //print("Calculated remaining distance to destination: " + remainingDistanceToTravel);

            if (remainingDistanceToTravel > stepDistance * 2)
            {
                Vector3 nextStep = Vector3.MoveTowards(dummyPosition, destinationPosition, stepDistance);
                stepPositionList.Add(nextStep);
                dummyPosition = nextStep;
            }
            else
            {
                // Now too close to final destination, so make the last step the final destination

                Vector3 finalStep = destinationPosition; 
                stepPositionList.Add(finalStep);
                dummyPosition = destinationPosition;
            }
        }
    }

    private void DisplaySteps(Vector3 destinationPosition)
    {
        //print("Number of steps to destination: " + stepPositionList.Count);

        foreach (Vector3 step in stepPositionList)
        {
            if (step == destinationPosition)
            {
                GameObject tmpEndStepReticle = GameObject.Instantiate(endStepReticle); // Duplicate our mid step object
                tmpEndStepReticle.transform.position = step;
                tmpEndStepReticle.SetActive(true);
                stepObjectsList.Add(tmpEndStepReticle);

            }
            else
            {
                GameObject tmpMidStepReticle = GameObject.Instantiate(midStepReticle); // Duplicate our mid step object
                tmpMidStepReticle.transform.position = step;
                tmpMidStepReticle.SetActive(true);
                stepObjectsList.Add(tmpMidStepReticle);
            }
        }
    }

    private void RemoveAllSteps()
    {
        // Visual objects

        foreach (GameObject stepObject in stepObjectsList)
        {
            Destroy(stepObject);
        }

        stepObjectsList.Clear();

        // Positions

        stepPositionList.Clear();

        // Should all be zero
        //print("Number of steps to destination: " + stepPositionList.Count);
        //print("Number of step objects to destination: " + stepObjectsList.Count);
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