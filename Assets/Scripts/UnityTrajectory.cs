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
            Debug.Log(" pos " + currentPointIndex);
        }
    }
    
    // Stop current animation
    public void StopTrajectory()
    {
        isAnimating = false;
    }
}
