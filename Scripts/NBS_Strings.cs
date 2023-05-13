using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBS_Strings : MonoBehaviour
{     
    // ********************************
    // String values to return
    // ********************************

    public string GetUIStringForKey(string key, string value)
    {
        if (key == "BLANK")
        {
            return "";
        }
        else if (key == "PressButtonToBegin")
        {
            string methodLetter = "Z";

            if (value == "ContinuousJoystick")
            {
                methodLetter = "1";
            }
            else if (value == "DirectTeleport_Instant")
            {
                methodLetter = "2";
            }
            else if (value == "DirectTeleport_Dash")
            {
                methodLetter = "3";
            }
            else if (value == "StepTeleport_Instant")
            {
                methodLetter = "4";
            }

            return "Press Button A to start test " + methodLetter;
        }
        else if (key == "Participant")
        {
            return "Participant ID: " + value;
        }
        else if (key == "RouteLoading")
        {
            return "Route completed.\n\nPlease wait...";
        }
        else if (key == "StudySceneLoading")
        {
            return "Loading study.\n\nPlease wait...";
        }
        else if (key == "CurrentRoute")
        {
            return "Route:" + value;
        }
        else if (key == "TimeLeft")
        {
            return value;
        }
        else if (key == "TimeExpired")
        {
            return "Time's up";
        }
        else if (key == "SceneEnd")
        {
            if (value == "Training")
            {
                return "End of tutorial";
            }
            else if (value == "Study")
            {
                return "End of study.\n\nThank you for participating.";
            }
        }
        else if (key == "StudyStart_Explain")
        {
            if (value == "Training")
            {
                return "\nFollow pink cylinders to the green destination endpoint. You have 90 seconds before the timer elapses.";
            }
            else if (value == "Study")
            {
                return "\nAnswer the researcher's question, then follow pink cylinders to the green destination endpoint. You have 90 seconds before the timer elapses.";
            }
        }
        else if (key == "StudyStart_NoExplain")
        {
            if (value == "Training")
            {
                return "\nMove to the same green destination endpoint again. You have 90 seconds before the timer elapses.";
            }
            else if (value == "Study")
            {
                return "\nAnswer the researcher's question, then move to the same green destination endpoint again. You have 90 seconds before the timer elapses.";
            }
        }
        else if (key == "StudyStart_How")
        {
            print("Value:" + value);

            if (value == "ContinuousJoystick")
            {
                return "to Look: Turn head or body\nto Move: Left joystick";
            }
            else if (value == "DirectTeleport_Instant")
            {
                return "to Look: Turn head or body\nto Move: Point left hand and pull bottom trigger";
            }
            else if (value == "DirectTeleport_Dash")
            {
                return "to Look: Turn head or body\nto Move: Point left hand and pull bottom trigger";
            }
            else if (value == "StepTeleport_Instant")
            {
                return "to Look: Turn head or body\nto Move: Point left hand and pull bottom trigger";
            }

            return "Method how error";
        }
        else if (key == "StudyEnd")
        {
            return "End of Study\n\nPlease remove headset";
        }
        else if (key == "SceneExplainer")
        {
            if (value == "Training") {

                return "Please practice moving and following the instructions";

            } else if (value == "Study")
            {
                return "Please read the study instructions";
            }
        }

        return "UI STRING ERROR";
    }

    public string GetLogStringForKey(string key)
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        if (key == "StudyState")
        {
            return "study_state";
        }
        else if (key == "PlayerInput")
        {
            return "player_input";
        }
        else if (key == "DidTeleport")
        {
            return "did_teleport";
        }
        else if (key == "RouteStepWaypointReached")
        {
            return "route_step_waypoint_reached";
        }
        else if (key == "RouteEndWaypointReached")
        {
            return "route_end_waypoint_reached";
        }
        else if (key == "SceneLoaded")
        {
            return "scene_loaded";
        }
        else if (key == "SceneEnded")
        {
            return "scene_ended";
        }
        else if (key == "RouteLoaded")
        {
            return "route_loaded";
        }
        else if (key == "RouteLoading")
        {
            return "route_loading";
        }
        else if (key == "WaitingOnPlayer")
        {
            return "waiting_for_player_to_start";
        }
        else if (key == "PlayerStarted")
        {
            return "player_started";
        }
        else if (key == "RouteTimeElapsed")
        {
            return "route_time_elapsed";
        }
        else if (key == "RouteTimeExpired")
        {
            return "route_time_expired";
        }
        else if (key == "SceneTimeElapsed")
        {
            return "scene_time_elapsed";
        }
        else if (key == "PlayerPosition")
        {
            return "player_position_xyz";
        }
        else if (key == "PlayerRotation")
        {
            return "player_rotation_xyz";
        }
        else if (key == "PlayerDistanceWaypoint")
        {
            return "player_distance_to_waypoint";
        }
        else if (key == "PlayerFacingWaypoint")
        {
            return "player_facing_waypoint";
        }
        else if (key == "RoutePlayerDistance")
        {
            return "player_route_distance_travelled";
        }        

        return "LOG STRING ERROR for key:" + key;
    }
}