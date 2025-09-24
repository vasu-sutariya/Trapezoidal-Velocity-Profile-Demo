using System.Collections.Generic;
using UnityEngine;

// Main MonoBehaviour that assigns trajectory to cube and handles animation
public class UnityTrajectory : MonoBehaviour
{
    [Header("Trajectory Configuration")]
    public TrajectoryParams trajectoryParams = new TrajectoryParams();
    
    [Header("Animation")]
    public bool autoStart = true;
    
    private List<TrajectoryPoint> trajectory;
    private int currentPointIndex = 0;
    private float startTime;
    private bool isAnimating = false;
    
    // Public properties for testing
    public List<TrajectoryPoint> Trajectory => trajectory;
    public int CurrentPointIndex => currentPointIndex;
    public float StartTime => startTime;
    public bool IsAnimating => isAnimating;
    
    void Start()
    {
        if (autoStart)
        {
            StartTrajectory();
        }
    }
    
    // Generate and start trajectory animation
    public void StartTrajectory()
    {
        trajectory = TrajectoryCalculator.GenerateTrajectory(trajectoryParams);
        
        if (trajectory == null || trajectory.Count == 0)
        {
            Debug.LogError("UnityTrajectory: trajectory is empty");
            isAnimating = false;
            return;
        }
        
        currentPointIndex = 0;
        startTime = Time.time;
        isAnimating = true;
        
        transform.position = trajectory[0].position;
    }
    
    void Update()
    {
        if (isAnimating) 
        {
        
            float elapsedTime = Time.time - startTime;
            
            while (currentPointIndex < trajectory.Count - 1 && 
                elapsedTime >= trajectory[currentPointIndex + 1].time)
            {
                currentPointIndex++;
            }
            
            if (currentPointIndex >= trajectory.Count - 1)
            {
                //  Complete
                transform.position = trajectory[trajectory.Count - 1].position;
                isAnimating = false;
                return;
            }
            
            // Assign position from trajectory list
            transform.position = trajectory[currentPointIndex].position;
            //Debug.Log(" pos " + currentPointIndex + " type " + trajectory[currentPointIndex].type);
        }
    }
    
    // Stop current animation
    public void StopTrajectory()
    {
        isAnimating = false;
    }
}
