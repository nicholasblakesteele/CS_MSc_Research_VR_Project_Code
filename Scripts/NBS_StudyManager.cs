using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NBS_StudyManager : MonoBehaviour
{   
    // Editor objects
    public NBS_PlayerManager playerManager;
    public NBS_RouteManager routeManager; // A pain to move into a script due to how collisions are handled, hence being a public drop in        
    public NBS_HUDManager hudManager; // For updating text on screen
    public Light mainLight; // For tutorial, for moving light
    public OVRCameraRig ovrCameraRig; // Headset and camera management

    // Tunable parameters
    private int maxRouteTimeAllowed = 90;
    private double batchLogEveryXMilliseconds = 50; // How often to log to memory. Must be 50 for study. 500 is good for testing to avoid massive logs
    private double writeLogEverySecondInMilliseconds = 1000; // How often to write to disk
    private float moveToNextSceneDelaySeconds = 5.0f;
    private float moveToNextRouteDelaySeconds = 1.5f;
    private float endSceneDelaySeconds = 3.0f;

    // State management    
    private bool routeIsEnding = false; // Stop multiple fires.
    private bool waypointEnabled = true; // NOTE: endpoint will always be visible even if this is set to false
    private bool studyIsPaused = false; // Let studyManager change this
    private bool randomGroupEnabled = false; // Alter in Start() not here
    public NBS_SceneState currentScene; // SET IN EDITOR for each scene. DO NOT CHANGE HERE 
    private int currentStep; // Let studyManager change this
    private int currentRoute; // Alter in Start() not here

    // Private classes for tracking states, save and input
    private NBS_SaveToDisk saveManager;
    private NBS_InputManager inputManager; // Used to access MenuManager also
    private NBS_Strings stringsManager;
    private NBS_RouteConfiguration routeConfiguration;
    private NBS_LocomotionState locomotionState;
    private NBS_LocomotionState previousLocomotionStateBeforePause;
    private NBS_Locomotion_TeleportionMovementType teleportationMovementType;           

    // Randomiser variables
    List<int> groupStartIndexList = new List<int>();

    // Track every n seconds for save
    private DateTime elapsedBatchLogSecondResetDt;
    private DateTime elapsedWriteLogSecondResetDt;
    private double elapsedBatchLogMillisecondsTime; // to track seconds accurately
    private double elapsedWriteLogMillisecondsTime; // to track seconds accurately

    // Track elapsed time
    private DateTime routeStartDt;
    private double totalElapsedTimeSeconds; // Excludes time waiting for loading times
    private double routeElapsedTimeSeconds;

    // Queue system for saving logs (to reduce IO overhead)
    private List<String> queueToWrite;
    private int userId; // Generated once when application starts. Used to help database manage different logs for each participant 
    private int sequenceID; // Used for helping database manage the order events happen (retro adding order is a pain in mysql)

    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        inputManager = playerManager.GetComponent<NBS_InputManager>();
        saveManager = this.GetComponent<NBS_SaveToDisk>();
        stringsManager = this.GetComponent<NBS_Strings>();
        routeConfiguration = this.GetComponent<NBS_RouteConfiguration>();
        queueToWrite = new List<String>();        
    }
    
    private void Start()
    {
        // Current scene should be set before this point
        if (currentScene != NBS_SceneState.Training && currentScene != NBS_SceneState.Study)
        {
            throw new Exception("ERROR - unknown scene at Start");
        }

        print("Loaded scene: " + currentScene);

        // Set userID
        if (currentScene == NBS_SceneState.Training)
        {
            // Start with a new user ID
            SetRandomIDForUser();

        } else
        {
            // Load user ID for Unity KVP
            // This assumes the user doing the Study scene is the same as the Training scene, which it should be
            RetrieveIDForExistingUser();
        }

        // Start log file before key variables are set (so first log is vanilla state)
        saveManager.InitialiseSavePath(); // Must always be first
        AddHeadersToWriteQueue();

        // Reset sequence ID
        ResetSequenceID();

        // Log scene start
        AddToWriteQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("SceneLoaded"));

        // Manage time
        routeStartDt = System.DateTime.Now;
        elapsedBatchLogSecondResetDt = System.DateTime.Now;
        elapsedWriteLogSecondResetDt = System.DateTime.Now;

        // Set key scene parameters

        SetRandomGroupEnabled(currentScene);

        if (randomGroupEnabled == true)
        {
            PopulateRandomiserGroups();
            currentRoute = GetRandomNextGroup();
        }
        else
        {
            currentRoute = 0; // Override if necessary (but groupEnabled must be false)
        }

        // Update HUD
        hudManager.UpdateLeftText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateCentreText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateRightText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateCentreText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateSceneText(GetUIStringForKey("BLANK", null));

        // Load route
        LoadRoute();
    }

    private void Update()
    {
        // This records certain information once per n seconds
        // Some types of information will be written as soon as it is ready

        elapsedBatchLogMillisecondsTime = (System.DateTime.Now - elapsedBatchLogSecondResetDt).TotalMilliseconds;
        elapsedWriteLogMillisecondsTime = (System.DateTime.Now - elapsedWriteLogSecondResetDt).TotalMilliseconds;

        if (elapsedBatchLogMillisecondsTime >= batchLogEveryXMilliseconds)
        {
            // Reset counts
            elapsedBatchLogMillisecondsTime = 0;
            elapsedBatchLogSecondResetDt = System.DateTime.Now;

            // Add to log ONLY if route is not ending

            if (routeIsEnding == false && studyIsPaused == false)
            {                
                AddToWriteQueue(GetLogStringForKey("PlayerPosition"), GetPlayerPosition());
                AddToWriteQueue(GetLogStringForKey("PlayerRotation"), GetPlayerRotation());
                AddToWriteQueue(GetLogStringForKey("PlayerDistanceWaypoint"), GetPlayerDistanceToWaypoint());
                AddToWriteQueue(GetLogStringForKey("PlayerFacingWaypoint"), GetPlayerIsFacingWaypoint());
            }
        }

        if (elapsedWriteLogMillisecondsTime >= writeLogEverySecondInMilliseconds)
        {
            // Update key variables ONLY if route is not ending

            if (routeIsEnding == false && studyIsPaused == false)
            {
                // Update elapsed time
                totalElapsedTimeSeconds += 1;
                routeElapsedTimeSeconds += 1;
            }

            elapsedWriteLogMillisecondsTime = 0;
            elapsedWriteLogSecondResetDt = System.DateTime.Now;

            // Write to disk all queued objects (incl. what classes have told us)
            DrainQueueByWritingToDisk();

        }

        // CheckRouteTimeRemaining must be here otherwise multiple frames will before code before end sequence completes

        if (routeIsEnding == false && studyIsPaused == false)
        {
            CheckRouteTimeRemaining();
        }
    }

    // ********************************
    // General scene methods
    // ********************************

    public void LoadRoute()
    {
        print("Loading route: " + currentRoute);

        // Important to prevent all movement until everything is loaded
        SetLocomotionState(NBS_LocomotionState.Disabled);
        inputManager.UpdateLocomotionControllers();

        AddToWriteQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("RouteLoading"));

        // Reasonable transition to try and reduce feeling sick
        playerManager.TriggerScreenFade();

        // Move lighting if needed
        MoveLightToNewPosition(currentScene, currentRoute);

        // Reset timer
        routeElapsedTimeSeconds = 0;
        routeIsEnding = false;

        // Reset route steps
        SetCurrentStep(0); // Be careful if overriding. Route configuration must be able to handle it   

        // Reset tracked values
        playerManager.ResetPlayedDistanceTravelled();

        // Load route (note: these variables should be set elsewhere)
        routeManager.StartCurrentRoute(currentScene, currentRoute, currentStep);

        // Set player variables based on scene
        routeConfiguration.GenerateStartingConfigurationForRoute(this, playerManager, currentScene, currentRoute);

        // Reset headset rotation for reach route (must be after route's configuration is loaded)
        ResetHeadsetRotation(playerManager.transform.eulerAngles.y);

        // Log level is now loaded
        AddToWriteQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("RouteLoaded"));
           
        // Route ready, but display 'press button to begin' for user to initiate movement and timer
        DisplayPressButtonToBegin();
    }

    private void ResetHeadsetRotation(float desiredHeadsetYRotation) 
    {
        // Reset headset rotation so user is facing the route's expected configured direction, not the headset's last rotation before the new route loaded

        float headsetYDifference = desiredHeadsetYRotation - ovrCameraRig.centerEyeAnchor.eulerAngles.y; ;
        ovrCameraRig.transform.Rotate(0, headsetYDifference, 0);
    }

    private void DisplayPressButtonToBegin()
    {
        AddToWriteQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("WaitingOnPlayer"));

        DisplayExplainerText(); // BEFORE pause otherwise will be set to disabled

        previousLocomotionStateBeforePause = GetLocomotionState();
        SetLocomotionState(NBS_LocomotionState.Disabled);
        inputManager.UpdateLocomotionControllers();

        studyIsPaused = true;
        
        hudManager.ShowBackgroundImage(); 
        hudManager.UpdateLeftText(GetUIStringForKey("Participant", userId.ToString()));
        hudManager.UpdateCentreText(GetUIStringForKey("PressButtonToBegin", previousLocomotionStateBeforePause.ToString()));
        hudManager.UpdateRightText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateSceneText(GetUIStringForKey("SceneExplainer", currentScene.ToString()));

        print("Paused");
    }

    private void DisplayExplainerText()
    {
        // Waypoint flag can be repurposed so we can alter instructions

        String currentLocomotionStateString = GetLocomotionState().ToString();
        String currentSceneString = currentScene.ToString();

        if (waypointEnabled == true)
        {
            hudManager.UpdateExplainerText(GetUIStringForKey("StudyStart_Explain", currentSceneString));
            hudManager.UpdateLocomotionText(GetUIStringForKey("StudyStart_How", currentLocomotionStateString));
        }
        else
        {
            hudManager.UpdateExplainerText(GetUIStringForKey("StudyStart_NoExplain", currentSceneString));
            hudManager.UpdateLocomotionText(GetUIStringForKey("StudyStart_How", currentLocomotionStateString));
        }
    }

    public void ButtonPressedToBegin()
    {
        SetLocomotionState(previousLocomotionStateBeforePause);
        inputManager.UpdateLocomotionControllers();

        AddToWriteQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("PlayerStarted"));

        hudManager.HideBackgroundImage();
        hudManager.UpdateLeftText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateCentreText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateRightText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateExplainerText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateLocomotionText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateSceneText(GetUIStringForKey("BLANK", null));

        playerManager.ResetPlayedDistanceTravelled();
        routeElapsedTimeSeconds = 0;
        studyIsPaused = false;
    }

    private void CheckRouteTimeRemaining()
    {
        double remainingTime = maxRouteTimeAllowed - routeElapsedTimeSeconds;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            routeIsEnding = true;
        }

        hudManager.UpdateLeftText(GetUIStringForKey("TimeLeft", remainingTime.ToString()));

        // Trigger end game caused by expiry

        if (routeIsEnding == true)
        {
            RouteTimeExpired();
        }
    }

    private void RouteTimeExpired()
    {
        AddToWriteQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("RouteTimeExpired"));

        DidReachEndPointForRoute();
    }

    // ********************************
    // End of route / scene methods
    // ********************************

    public void DidReachEndPointForRoute()
    {
        print("Reached end. Current route: " + currentRoute);

        // NOTE: Do not do logging here, as could be several reasons this is called

        if (GetRandomGroupEnabled() == true)
        {
            if (IsGroupListEmpty() == true && IsEven(currentRoute) == false)
            {
                ReachedEndOfScene();
            }
            else
            {
                ReachedEndOfRoute();
            }
        }
        else
        {
            if (currentRoute == routeConfiguration.GetNumberOfRoutesForScene(currentScene))
            {
                ReachedEndOfScene();
            }
            else
            {
                ReachedEndOfRoute();
            }
        }
    }

    public void ReachedEndOfRoute()
    {
        routeIsEnding = true;

        AddToWriteQueue(GetLogStringForKey("RouteTimeElapsed"), routeElapsedTimeSeconds.ToString());
        AddToWriteQueue(GetLogStringForKey("RoutePlayerDistance"), GetPlayerDistanceTravelled());

        hudManager.ShowBackgroundImage();
        hudManager.UpdateLeftText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateExplainerText(GetUIStringForKey("RouteLoading", null));
        hudManager.UpdateRightText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateCentreText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateLocomotionText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateSceneText(GetUIStringForKey("BLANK", null));

        // Disable player AFTER logging is complete
        SetLocomotionState(NBS_LocomotionState.Disabled);
        inputManager.UpdateLocomotionControllers();

        // Update variables to move on

        if (randomGroupEnabled == true)
        {
            if (IsEven(currentRoute) == true)
            {
                // As is even, one more route left is this group

                currentRoute += 1; // Always incremeent by 1. RouteManager will handle if over

            }
            else
            {
                // Group has expired - moving onto next group

                currentRoute = GetRandomNextGroup();
            }
        }
        else
        {
            // No random selection - just going through routes in order incrementally

            currentRoute += 1; // Always incremeent by 1. RouteManager will handle if over
        }

        // Move to loading next route
        Invoke(nameof(LoadRoute), moveToNextRouteDelaySeconds);
    }

    public void ReachedEndOfScene()
    {
        routeIsEnding = true;

        AddToWriteQueue(GetLogStringForKey("RouteTimeElapsed"), routeElapsedTimeSeconds.ToString());
        AddToWriteQueue(GetLogStringForKey("RoutePlayerDistance"), GetPlayerDistanceTravelled());

        // Disable player AFTER route logging is complete to ensure it is accurate
        SetLocomotionState(NBS_LocomotionState.Disabled);
        inputManager.UpdateLocomotionControllers();

        AddToWriteQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("SceneEnded"));
        AddToWriteQueue(GetLogStringForKey("SceneTimeElapsed"), totalElapsedTimeSeconds.ToString());

        hudManager.ShowBackgroundImage();
        hudManager.UpdateLeftText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateExplainerText(GetUIStringForKey("SceneEnd", currentScene.ToString()));
        hudManager.UpdateRightText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateCentreText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateSceneText(GetUIStringForKey("BLANK", null));

        print("Finished scene");

        Invoke(nameof(MoveToNextScene), endSceneDelaySeconds);
    }

    private void MoveToNextScene()
    {
        hudManager.ShowBackgroundImage();
        hudManager.UpdateLeftText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateRightText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateExplainerText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateLocomotionText(GetUIStringForKey("BLANK", null));
        hudManager.UpdateSceneText(GetUIStringForKey("BLANK", null));

        if (currentScene == NBS_SceneState.Training)
        {
            hudManager.UpdateExplainerText(GetUIStringForKey("StudySceneLoading", null));

            Invoke(nameof(ChangeToStudyScene), moveToNextSceneDelaySeconds);
        }
        else if (currentScene == NBS_SceneState.Study)
        {
            hudManager.UpdateCentreText(GetUIStringForKey("StudyEnd", null));
        }
        else
        {
            throw new Exception("ERROR - unknown scene at MoveToNextScene");
        }
    }

    // ********************************
    // Randomiser methods
    // ********************************

    private void SetRandomGroupEnabled(NBS_SceneState scene)
    {
        if (scene == NBS_SceneState.Training)
        {
            randomGroupEnabled = false;
        }
        else if (scene == NBS_SceneState.Study)
        {
            randomGroupEnabled = true; 
        }
        else
        {
            throw new Exception("ERROR - unknown scene at SetRandomGroupEnabled");
        }
    }

    private void PopulateRandomiserGroups()
    {
        // Even is always start of group. Even = guided, odd = unguided
        // (e.g. 1 = 0,1; 2 = 2,3)

        for (int i = 0; i < GetNumberOfGroupsForScene(currentScene) * 2; i += 2)
        {
            groupStartIndexList.Add(i);
        }
    }

    /*
    private void IterateGroupStartIndexList()
    {
        foreach (int i in groupStartIndexList)
        {
            print("i:" + i);
        }
    }
    */

    private int GetRandomNextGroup()
    {
        int numberOfGroupsLeft = groupStartIndexList.Count;

        int randomIndex = UnityEngine.Random.Range(0, numberOfGroupsLeft);

        int randomNumber = groupStartIndexList[randomIndex];

        // Remove selected group from groupStartIndexList
        groupStartIndexList.RemoveAt(randomIndex);

        return randomNumber;
    }

    public bool IsGroupListEmpty()
    {
        if (groupStartIndexList.Count == 0)
        {
            return true;
        }

        return false;
    }

    private int GetNumberOfGroupsForScene(NBS_SceneState scene)
    {
        return (routeConfiguration.GetNumberOfRoutesForScene(scene) + 1) / 2;
    }

    public bool IsEven(int n)
    {
        if (n % 2 == 0)
        {
            return true;
        }

        return false;
    }

    // ********************************
    // Queue methods for saving to log and associated methods
    // ********************************

    private void DrainQueueByWritingToDisk()
    {
        if (queueToWrite.Count > 0)
        {
            // The log builds up as needed, but is only saved to disk when update deems it necessary (currently time based)
            saveManager.WriteToLogFile(queueToWrite);
        }

        // Clear queue log
        queueToWrite.Clear();
    }
    
    public void AddToWriteQueue(string category, string value)
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        if (value.Contains(",") == false)
        {
            value += ",,,"; // Pad out for xyz
        }

        queueToWrite.Add(GetCurrentUserID() + "," + sequenceID + "," + currentScene + "," + currentRoute + "," + currentStep + "," + GetLocomotionState() + "," + waypointEnabled + "," + category + "," + value);

        UpdateSequenceID();
    }

    private void AddHeadersToWriteQueue()
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        queueToWrite.Add("user_id,sequence_id,current_scene,current_route,current_step,current_locomotion,waypoint_enabled,category,value,x,y,z");
    }

    private void ResetSequenceID()
    {
        sequenceID = 1;
    }

    private void UpdateSequenceID()
    {
        sequenceID += 1;
    }

    private void SetRandomIDForUser()
    {
        // This is not guaranteed to be unique, but as we're using 8 digits and expect to only have 10-20 participants, the risk of clashes is acceptable

        int randomId = UnityEngine.Random.Range(10000000, 99999999);
        
        userId = randomId;

        SaveUserIDToPersistentStorage(); 
    }

    private int GetCurrentUserID()
    {
        return userId;
    }

    private void RetrieveIDForExistingUser()
    {
        // Use Unity's build in KVP system to manage userID across scenes
        // LoadScene() must handle when this needs to be reset

        userId = PlayerPrefs.GetInt("UserID", 0);
    }

    private void SaveUserIDToPersistentStorage()
    {
        // Use Unity's build in KVP system to manage userID across scenes
        // LoadScene() must handle when this needs to be reset

        PlayerPrefs.SetInt("UserID", GetCurrentUserID());
        PlayerPrefs.Save();
    }

    // ************************
    // State methods
    // ************************

    public bool GetRandomGroupEnabled()
    {
        return randomGroupEnabled;
    }

    public bool GetStudyPauseStatus()
    {
        return studyIsPaused;
    }

    public NBS_LocomotionState GetLocomotionState()
    {
        return locomotionState;
    }

    public void SetLocomotionState(NBS_LocomotionState newState)
    {
        locomotionState = newState;
    }

    public void SetWaypointState(bool isEnabled)
    {
        waypointEnabled = isEnabled;

        // NOTE: endpoint will always be visible even if this is set to false
    }

    public void SetCurrentStep(int step)
    {
        currentStep = step;
    }

    private string GetPlayerDistanceToWaypoint()
    {
        // Can be waypoint or endpoint
        Vector3 playerPosition = playerManager.GetPlayerPosition();
        Vector3 waypointPosition = routeManager.GetCurrentWaypointTargetPosition();

        float distanceToWaypointTarget = Vector3.Distance(playerPosition, waypointPosition);
        string distanceToWaypointTargetString = distanceToWaypointTarget.ToString();

        return distanceToWaypointTargetString;
    }

    private string GetPlayerIsFacingWaypoint()
    {
        // Note this does NOT mean the player can see the waypoint
        // User can be facing the right direction and the object can be hidden behind something else

        Transform playerTransform = playerManager.GetPlayerTransform();
        Vector3 playerPosition = playerManager.GetPlayerPosition();
        Vector3 waypointPosition = routeManager.GetCurrentWaypointTargetPosition();

        float dot = Vector3.Dot(playerManager.transform.forward, (waypointPosition - playerPosition).normalized);

        if (dot > 0.8f)
        { // The closer to 1, the more directly in front of the player does it need to be         

            return true.ToString();

        }
        else
        {
            return false.ToString();
        }
    }

    private string GetPlayerPosition()
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        Vector3 position = playerManager.GetPlayerPosition();
        string positionString = "," + Math.Round(position.x, 3) + "," + Math.Round(position.y, 3) + "," + Math.Round(position.z, 3).ToString();
        return positionString;
    }

    private string GetPlayerRotation()
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        Vector3 rotation = playerManager.GetPlayerRotation();
        string rotationString = "," + Math.Round(rotation.x, 3) + "," + Math.Round(rotation.y, 3) + "," + Math.Round(rotation.z, 3).ToString();
        return rotationString;
    }

    private string GetPlayerDistanceTravelled()
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        float distance = playerManager.GetPlayerDistanceTravelled();
        string distanceString = Math.Round(distance, 3).ToString();
        return distanceString;
    }

    /*
    private string GetCurrentDateTimeAsSQLDatetimeString()
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }
    */

    private string GetUIStringForKey(string key, string value)
    {
        return stringsManager.GetUIStringForKey(key, value);
    }
    
    private string GetLogStringForKey(string key)
    {
        // NOTE: Do not add new lines or carriage returns. Save class will handle these

        return stringsManager.GetLogStringForKey(key);
    } 
    
    // ************************
    // Scene methods
    // NOTE: likely don't need to be in this class, but leave for now
    // ************************

    private void ChangeToTutorialScene()
    {
        print("Changing to Tutorial scene");
        SceneManager.LoadScene("Tutorial_Scene");
    }

    private void ChangeToStudyScene()
    {
        print("Changing to Study scene");
        SceneManager.LoadScene("Study_Scene");
    }

    public void MoveLightToNewPosition(NBS_SceneState scene, int route)
    {
        // Bit of a manual approach to make lighting appear OK without lots of lights in the scene

        float yHeight = 6.0f;

        if (scene == NBS_SceneState.Training)
        {
            if (route == 0 || route == 1)
            {
                mainLight.transform.position = new Vector3(-2.5f, yHeight, 2.5f);
            }
            else if (route == 2 || route == 3)
            {
                mainLight.transform.position = new Vector3(2.5f, yHeight, 2.5f);
            }
            else if (route == 4 || route == 5)
            {
                mainLight.transform.position = new Vector3(-2.5f, yHeight, -2.5f);
            }
            else if (route == 6 || route == 7)
            {
                mainLight.transform.position = new Vector3(2.5f, yHeight, -2.5f);
            }
        }

        // Currently not doing any light movement in the study scene
    }
}