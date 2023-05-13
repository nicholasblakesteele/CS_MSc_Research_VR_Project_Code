using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBS_RouteManager : MonoBehaviour
{
    // Editor objects
    public NBS_StudyManager studyManager; // For accessing logs
    public Material[] materials;

    // Convenience variables
    private List<Vector3> waypointList;
    private MeshRenderer waypointRenderer;
    private NBS_Strings stringsManager;
    private NBS_RouteConfiguration routeConfiguration;
    private Vector3 currentWaypointTarget; // Does NOT set the actual postition, it's just so other classes can easily access this if needed

    // These 3 class variables only exist due to the physics trigger which can't access them otherwise
    // They do NOT determine the state - they are overwritten by StudyManager
    // All methods should pass these variables as arguments 
    private NBS_SceneState scene; 
    private int route; 
    private int step; 

    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        waypointRenderer = this.GetComponent<MeshRenderer>();
        stringsManager = this.GetComponent<NBS_Strings>();
        routeConfiguration = this.GetComponent<NBS_RouteConfiguration>();
        waypointList = new List<Vector3>();
    }

    void Start()
    {
        // Not used
    }

    private void Update()
    {
        // Not used
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle physics collision - use onTrigger not onCollide

        if (other.gameObject.tag == "Player")
        {
            print("Player reached waypoint");

            DidReachWaypoint(); // Note: use of class variables as trigger doesn't now these
        }
    }

    // ********************************
    // Route set up methods
    // ********************************

    public void StartCurrentRoute(NBS_SceneState _scene, int _route, int _step)
    {
        // Deliberately cannot pass in the current route to avoid accidentally trigering a route change
        // Must set first using SetCurrentRoute then call this method

        scene = _scene;
        route = _route;
        step = _step;

        waypointList.Clear(); // Important! Always clear prior loaded values should a previous route be loaded

        waypointList = routeConfiguration.GenerateWaypointsForRoute(scene, route);
        
        DisplayNextWaypoint(step);

        ShowWaypoints();
    }

    private void HideWaypoints()
    {
        this.gameObject.SetActive(false);
    }    

    private void ShowWaypoints()
    {
        this.gameObject.SetActive(true);
    }

    // ********************************
    // Waypoint methods
    // ********************************

    private void DidReachWaypoint()
    {        
        if (step == waypointList.Count - 1)
        {
            // End of route

            AddToStudyManagerLogQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("RouteEndWaypointReached"));

            DidReachEndPointForRoute();            

        } else
        {
            AddToStudyManagerLogQueue(GetLogStringForKey("StudyState"), GetLogStringForKey("RouteStepWaypointReached"));

            step += 1;

            DisplayNextWaypoint(step);
        }
    }
    
    private void DidReachEndPointForRoute()
    {
        // Hide all to prevent confusion
        HideWaypoints();

        studyManager.DidReachEndPointForRoute();       
    }

    private void DisplayNextWaypoint(int _step)
    {
        if (_step == waypointList.Count - 1)
        {
            // Update material to end point
            waypointRenderer.sharedMaterial = materials[1];

            // Update position of waypoint
            Vector3 endVector = waypointList[_step];
            this.transform.position = endVector;
            currentWaypointTarget = endVector;
        }
        else if (_step > waypointList.Count - 1)
        {
            // Error with set up
            print("Incorrect next step added - more than in list:" + _step);
        }
        else
        {
            // Update material to waypoint
            waypointRenderer.sharedMaterial = materials[0];

            // Update position of waypoint
            Vector3 nextVector = waypointList[_step];
            this.transform.position = nextVector;
            currentWaypointTarget = nextVector;
        }

        // Update studyManager for logging

        UpdateStudyManagerStep(_step);
    }

    // ************************
    // Convenience methods
    // ************************

    public Vector3 GetCurrentWaypointTargetPosition()
    {
        return currentWaypointTarget; 
    }

    private string GetLogStringForKey(string key)
    {
        return stringsManager.GetLogStringForKey(key);
    }

    private void AddToStudyManagerLogQueue(string type, string value)
    {
        studyManager.AddToWriteQueue(type, value);
    }

    private void UpdateStudyManagerStep(int step)
    {
        studyManager.SetCurrentStep(step);
    }
}