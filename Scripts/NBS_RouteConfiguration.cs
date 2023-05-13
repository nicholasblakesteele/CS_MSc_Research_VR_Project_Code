using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBS_RouteConfiguration : MonoBehaviour
{
    public int GetNumberOfRoutesForScene(NBS_SceneState scene)
    {
        // NOTE: Manually update GetNumberOfRoutesForScene() if adjusting number of routes in other methods in this class

        if (scene == NBS_SceneState.Training)
        {
            int n = 8; // Actual number of routes
            return n - 1; // Max index number
        }
        else if (scene == NBS_SceneState.Study)
        {
            int n = 8; // Actual number of routes. 
            return n - 1; // Max index number 
        }
        else
        {
            throw new Exception("ERROR - unknown scene at GetNumberOfRoutesForScene");
        }        
    }

    public void GenerateStartingConfigurationForRoute(NBS_StudyManager studyManager, NBS_PlayerManager playerManager, NBS_SceneState scene, int route)
    {
        // NOTE: must be even number of routes (e.g. each group = a pair of routes)
        // REMEMBER: spawn location should always be able to see a waypoint (if relevant)

        // Fix y height to minimise risk of setting player at wrong height
        float yHeight = 0.0f; 

        if (scene == NBS_SceneState.Training)
        {           
            // ----------
            // Group 1
            // ----------

            if (route == 0)
            {
                playerManager.SetPlayerPosition(new Vector3(-4.0f, yHeight, 1.5f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 25, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.ContinuousJoystick);
                studyManager.SetWaypointState(true);
            }
            else if (route == 1)
            {
                // Repeat without pointers

                playerManager.SetPlayerPosition(new Vector3(-4.0f, yHeight, 1.5f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 25, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.ContinuousJoystick);
                studyManager.SetWaypointState(false);
            }

            // ----------
            // Group 2
            // ----------

            else if (route == 2)
            {
                playerManager.SetPlayerPosition(new Vector3(1.0f, yHeight, 1.3f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 45, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Instant);
                studyManager.SetWaypointState(true);
            }

            else if (route == 3)
            {
                // Repeat without pointers

                playerManager.SetPlayerPosition(new Vector3(1.0f, yHeight, 1.3f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 45, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Instant);
                studyManager.SetWaypointState(false);
            }

            // ----------
            // Group 3
            // ----------

            else if (route == 4)
            {
                playerManager.SetPlayerPosition(new Vector3(-2.5f, yHeight, -2.5f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -35, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Dash);
                studyManager.SetWaypointState(true);
            }

            else if (route == 5)
            {
                // Repeat without pointers

                playerManager.SetPlayerPosition(new Vector3(-2.5f, yHeight, -2.5f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -35, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Dash);
                studyManager.SetWaypointState(false);
            }

            // ----------
            // Group 4
            // ----------

            else if (route == 6) 
            {
                playerManager.SetPlayerPosition(new Vector3(4.0f, yHeight, -1.0f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -80, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.StepTeleport_Instant); 
                studyManager.SetWaypointState(true);
            }

            else if (route == 7)
            {
                // Repeat without pointers

                playerManager.SetPlayerPosition(new Vector3(4.0f, yHeight, -1.0f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -80, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.StepTeleport_Instant);
                studyManager.SetWaypointState(false);
            }
        }

        else if (scene == NBS_SceneState.Study)
        {
            // ----------
            // Group 1
            // ----------

            if (route == 0)
            {
                playerManager.SetPlayerPosition(new Vector3(-3.2f, yHeight, -11.0f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 10, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.ContinuousJoystick);
                studyManager.SetWaypointState(true);
            }
            else if (route == 1)
            {
                // Repeat without waypoints

                playerManager.SetPlayerPosition(new Vector3(-3.2f, yHeight, -11.0f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 10, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.ContinuousJoystick);
                studyManager.SetWaypointState(false);
            }

            // ----------
            // Group 2
            // ----------

            if (route == 2)
            {
                playerManager.SetPlayerPosition(new Vector3(12.97f, yHeight, 13.3f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -117, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Instant);
                studyManager.SetWaypointState(true);
            }
            else if (route == 3)
            {
                // Repeat without waypoints

                playerManager.SetPlayerPosition(new Vector3(12.97f, yHeight, 13.3f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -117, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Instant);
                studyManager.SetWaypointState(false);
            }

            // ----------
            // Group 3
            // ----------

            if (route == 4)
            {
                playerManager.SetPlayerPosition(new Vector3(24.3f, yHeight, 8.97f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -123, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Dash);
                studyManager.SetWaypointState(true);
            }
            else if (route == 5)
            {
                // Repeat without waypoints

                playerManager.SetPlayerPosition(new Vector3(24.3f, yHeight, 8.97f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, -123, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.DirectTeleport_Dash);
                studyManager.SetWaypointState(false);
            }

            // ----------
            // Group 4
            // ----------

            if (route == 6)
            {
                playerManager.SetPlayerPosition(new Vector3(3.63f, yHeight, 10.5f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 142, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.StepTeleport_Instant);
                studyManager.SetWaypointState(true);
            }
            else if (route == 7)
            {
                // Repeat without waypoints

                playerManager.SetPlayerPosition(new Vector3(3.63f, yHeight, 10.5f));
                playerManager.SetPlayerEulerRotation(new Vector3(0, 142, 0));
                studyManager.SetLocomotionState(NBS_LocomotionState.StepTeleport_Instant);
                studyManager.SetWaypointState(false);
            }

        }

        else
        {
            throw new Exception("ERROR - unknown scene at GenerateStartingConfigurationForRoute");
        }

        // Update variables regardless of scene / route

        playerManager.SetStartPositionForDistanceCalculation(playerManager.GetPlayerPosition());        
    }

    public List<Vector3> GenerateWaypointsForRoute(NBS_SceneState scene, int route)
    {
        // NOTE: must be even number of routes (e.g. each group = a pair of routes)
        // REMEMBER: spawn location should always be able to see a waypoint (if relevant)

        List<Vector3> waypointList = new List<Vector3>();

        // Fix y height to minimise risk of setting waypoint at wrong height
        float yHeight = 0.5f;

        if (scene == NBS_SceneState.Training)
        {
            // ----------
            // Group 1
            // ----------

            if (route == 0)
            {
                Vector3 waypoint1 = new Vector3(-4.0f, yHeight, 4.0f);
                Vector3 waypoint2 = new Vector3(-1.6f, yHeight, 4.0f);
                Vector3 endpoint = new Vector3(-1.0f, yHeight, 2.0f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(endpoint);
            }

            else if (route == 1) // Endpoint only
            {                                
                Vector3 endpoint = new Vector3(-1.0f, yHeight, 2.0f);
                
                waypointList.Add(endpoint);
            }

            // ----------
            // Group 2
            // ----------

            else if (route == 2)
            {

                Vector3 waypoint1 = new Vector3(1.1f, yHeight, 3.6f);
                Vector3 waypoint2 = new Vector3(3.6f, yHeight, 3.6f);
                Vector3 endpoint = new Vector3(4.0f, yHeight, 1.1f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(endpoint);
            }

            else if (route == 3) // Endpoint only
            {                
                Vector3 endpoint = new Vector3(4.0f, yHeight, 1.1f);
               
                waypointList.Add(endpoint);
            }

            // ----------
            // Group 3
            // ----------

            else if (route == 4)
            {
                Vector3 waypoint1 = new Vector3(-4.0f, yHeight, -1.2f);
                Vector3 waypoint2 = new Vector3(-4.2f, yHeight, -4.3f);
                Vector3 waypoint3 = new Vector3(-1.4f, yHeight, -1.2f);
                Vector3 endpoint = new Vector3(-0.9f, yHeight, -4.2f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(waypoint3);
                waypointList.Add(endpoint);

            }
            else if (route == 5) // Endpoint only
            {                
                Vector3 endpoint = new Vector3(-0.9f, yHeight, -4.2f);
                
                waypointList.Add(endpoint);
            }

            // ----------
            // Group 4
            // ----------

            else if (route == 6)
            {
                Vector3 waypoint1 = new Vector3(0.9f, yHeight, -0.9f);
                Vector3 waypoint2 = new Vector3(0.9f, yHeight, -4.15f);
                Vector3 waypoint3 = new Vector3(4.15f, yHeight, -4.15f);
                Vector3 waypoint4 = new Vector3(0.9f, yHeight, -4.15f);
                Vector3 waypoint5 = new Vector3(0.9f, yHeight, -2.55f);
                Vector3 endpoint = new Vector3(2.0f, yHeight, -2.55f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(waypoint3);
                waypointList.Add(waypoint4);
                waypointList.Add(waypoint5);
                waypointList.Add(endpoint);
            }
            else if (route == 7) // Endpoint only
            {
                Vector3 endpoint = new Vector3(2.0f, yHeight, -2.55f);
               
                waypointList.Add(endpoint);
            }

            else
            {
                throw new Exception("ERROR - unknown Tutorial route at GenerateWaypointsForRoute");
            }
        }
        
        else if (scene == NBS_SceneState.Study)
        {
            // ----------
            // Group 1
            // ----------

            if (route == 0)
            {
                Vector3 waypoint1 = new Vector3(-1.15f, yHeight, -4.5f);
                Vector3 waypoint2 = new Vector3(7.4f, yHeight, 1.48f);
                Vector3 waypoint3 = new Vector3(13.8f, yHeight, 5.3f);
                Vector3 waypoint4 = new Vector3(18.86f, yHeight, 0.6f);
                Vector3 waypoint5 = new Vector3(19.9f, yHeight, -4.69f);
                Vector3 endpoint = new Vector3(22.53f, yHeight, -8.79f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(waypoint3);
                waypointList.Add(waypoint4);
                waypointList.Add(waypoint5);
                waypointList.Add(endpoint);
            }
            else if (route == 1) // Endpoint only
            {
                Vector3 endpoint = new Vector3(22.53f, yHeight, -8.79f);

                waypointList.Add(endpoint);
            }

            // ----------
            // Group 2
            // ----------

            else if (route == 2)
            {
                Vector3 waypoint1 = new Vector3(5.28f, yHeight, 9.86f);
                Vector3 waypoint2 = new Vector3(-1.613f, yHeight, 4.92f);
                Vector3 waypoint3 = new Vector3(-4.52f, yHeight, -7.57f);
                Vector3 waypoint4 = new Vector3(-0.17f, yHeight, -7.47f);
                Vector3 waypoint5 = new Vector3(7.47f, yHeight, -2.14f);
                Vector3 endpoint = new Vector3(12.32f, yHeight, -7.82f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(waypoint3);
                waypointList.Add(waypoint4);
                waypointList.Add(waypoint5);
                waypointList.Add(endpoint);
            }
            else if (route == 3) // Endpoint only
            {
                Vector3 endpoint = new Vector3(12.32f, yHeight, -7.82f);

                waypointList.Add(endpoint);
            }

            // ----------
            // Group 3
            // ----------

            else if (route == 4)
            {
                Vector3 waypoint1 = new Vector3(15.5f, yHeight, 4.76f);
                Vector3 waypoint2 = new Vector3(-4.68f, yHeight, 5.9f);
                Vector3 waypoint3 = new Vector3(-7.24f, yHeight, 12.91f);
                Vector3 waypoint4 = new Vector3(-13.4f, yHeight, 10.58f);
                Vector3 waypoint5 = new Vector3(-12.24f, yHeight, -0.49f);
                Vector3 endpoint = new Vector3(-8.31f, yHeight, -3.17f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(waypoint3);
                waypointList.Add(waypoint4);
                waypointList.Add(waypoint5);
                waypointList.Add(endpoint);
            }
            else if (route == 5) // Endpoint only
            {
                Vector3 endpoint = new Vector3(-8.31f, yHeight, -3.17f);

                waypointList.Add(endpoint);
            }

            // ----------
            // Group 4
            // ----------

            else if (route == 6)
            {
                Vector3 waypoint1 = new Vector3(10.89f, yHeight, 1.5f);
                Vector3 waypoint2 = new Vector3(17.62f, yHeight, -0.75f);
                Vector3 waypoint3 = new Vector3(17.2f, yHeight, -4.67f);
                Vector3 waypoint4 = new Vector3(9.0f, yHeight, -3.59f);
                Vector3 waypoint5 = new Vector3(-2.46f, yHeight, -3.18f);
                Vector3 endpoint = new Vector3(-10.83f, yHeight, -9.60f);

                waypointList.Add(waypoint1);
                waypointList.Add(waypoint2);
                waypointList.Add(waypoint3);
                waypointList.Add(waypoint4);
                waypointList.Add(waypoint5);
                waypointList.Add(endpoint);
            }
            else if (route == 7) // Endpoint only
            {
                Vector3 endpoint = new Vector3(-10.83f, yHeight, -9.60f);

                waypointList.Add(endpoint);
            }

            else
            {
                throw new Exception("ERROR - unknown Study route at GenerateWaypointsForRoute");
            }
        }

        else
        {
            throw new Exception("ERROR - unknown scene at GenerateWaypointsForRoute");
        }

        // Sense check is even

        return waypointList;
    }
}