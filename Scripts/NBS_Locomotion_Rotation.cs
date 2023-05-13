using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBS_Locomotion_Rotation: MonoBehaviour
{
    private bool isEnabled = false; 
    private float newYRotationValue = 0;

    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        // Not used
    }

    private void Start()
    {
        // Not used
    }

    private void Update()
    {                
        this.transform.rotation = Quaternion.Euler(0.0f, this.transform.rotation.eulerAngles.y + newYRotationValue, 0.0f);        
    }
   
    // ********************************
    // Movement methods
    // ********************************

    public void RotatePlayer(float inputValue)
    {
        // Disable the ability to update the rotation amount here and not in UPDATE        

        if (isEnabled == true)
        {
            newYRotationValue = inputValue;

            // Update will handle actual movement           
        }
    }

    // ********************************
    // Accessor methods
    // ********************************

    public void SetNBSEnabled(bool _isEnabled)
    {
        isEnabled = _isEnabled;
    }
}