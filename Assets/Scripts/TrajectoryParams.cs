using UnityEngine;

// Parameters for trapezoidal velocity profile trajectory generation
[System.Serializable]
public class TrajectoryParams
{
    // Starting position
    public Vector3 startPoint = Vector3.zero;
    
    // Ending position
    public Vector3 endPoint = Vector3.forward * 10f;
    
    // Maximum velocity during constant phase
    public float maxVelocity = 5f;
    
    // Acceleration during ramp-up phase
    public float acceleration = 2f;
    
    // Deceleration during ramp-down phase
    public float deceleration = 2f;
    
    // Time interval between trajectory points
    public float samplingInterval = 0.1f;
}
