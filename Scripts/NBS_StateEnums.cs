using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NBS_LocomotionState
{
    Disabled,
    ContinuousJoystick,
    DirectTeleport_Instant,
    DirectTeleport_Dash,
    StepTeleport_Instant
}

public enum NBS_Locomotion_TeleportionMovementType
{
    Instant,
    Dash
}

public enum NBS_SceneState
{    
    Training,
    Study
}