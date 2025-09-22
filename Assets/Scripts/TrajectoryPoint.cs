using UnityEngine;

// Data structure representing a single point on the trajectory
[System.Serializable]
public struct TrajectoryPoint
{
    // Position in 3D space
    public Vector3 position;
    
    // Velocity at this point
    public float velocity;
    
    // Time from trajectory start
    public float time;
    
    // Type of velocity profile phase
    public string type;
    
    public TrajectoryPoint(Vector3 position, float velocity, float time, string type)
    {
        this.position = position;
        this.velocity = velocity;
        this.time = time;
        this.type = type;
    }
}
