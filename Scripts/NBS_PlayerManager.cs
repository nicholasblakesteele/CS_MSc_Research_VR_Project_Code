using UnityEngine;

public class NBS_PlayerManager : MonoBehaviour
{
    // Editor objects
    public OVRScreenFade screenFade;

    // State variables
    private float distanceTravelled = 0;
    private Vector3 previousPosition; 

    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        // Not used
    }

    void Start()
    {
        // Not used
    }

    void Update()
    {
        CalculateDistanceTravelled();

        previousPosition = GetPlayerPosition();
    }

    // ********************************
    // Calculated methods
    // ********************************

    private void CalculateDistanceTravelled()
    {
        distanceTravelled += Vector3.Distance(previousPosition, GetPlayerPosition());
        
        //print("Distance travelled:" + distanceTravelled);
    }

    // ********************************
    // Accessor methods
    // ********************************

    public void TriggerScreenFade()
    {
        screenFade.FadeIn();
    }

    public void SetPlayerPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public void SetStartPositionForDistanceCalculation(Vector3 position)
    {
        previousPosition = position;
    }

    public Vector3 GetPlayerPosition()
    {
        return this.transform.position; 
    }

    public void SetPlayerEulerRotation(Vector3 eulerRotation)
    {
        this.transform.eulerAngles = eulerRotation;
    }

    public Vector3 GetPlayerRotation()
    {
        return this.transform.eulerAngles; // or .rotation
    }

    public Transform GetPlayerTransform()
    {
        return this.transform;
    }
       
    public float GetPlayerDistanceTravelled()
    {
        return distanceTravelled; 
    }

    public void ResetPlayedDistanceTravelled()
    {
        distanceTravelled = 0; 
    }
}