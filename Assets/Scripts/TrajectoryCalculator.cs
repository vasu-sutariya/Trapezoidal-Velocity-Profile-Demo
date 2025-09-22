using System.Collections.Generic;
using UnityEngine;

// Calculates trapezoidal velocity profile trajectory points
public static class TrajectoryCalculator
{
    // Generate trajectory points using trapezoidal velocity profile
    public static List<TrajectoryPoint> GenerateTrajectory(TrajectoryParams parameters)
    {
        List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();
        
        float distance = Vector3.Distance(parameters.startPoint, parameters.endPoint);
        Vector3 direction = (parameters.endPoint - parameters.startPoint).normalized;
 
        // Calculate phase durations and distances
        float accelTime = parameters.maxVelocity / parameters.acceleration;
        float decelTime = parameters.maxVelocity / parameters.deceleration;
        float accelDistance = 0.5f * parameters.acceleration * accelTime * accelTime;
        float decelDistance = 0.5f * parameters.deceleration * decelTime * decelTime;
  
        if (accelDistance + decelDistance > distance)
        {
            // triangular profile
            float peakVelocity = Mathf.Sqrt(2f * distance * parameters.acceleration * parameters.deceleration / 
                                           (parameters.acceleration + parameters.deceleration));
            accelTime = peakVelocity / parameters.acceleration;
            decelTime = peakVelocity / parameters.deceleration;
            accelDistance = 0.5f * parameters.acceleration * accelTime * accelTime;
            decelDistance = distance - accelDistance;
        }
        
        float constantDistance = distance - accelDistance - decelDistance;
        float constantTime = constantDistance / parameters.maxVelocity;
        float totalTime = accelTime + constantTime + decelTime;
        
        // Generate trajectory points
        for (float t = 0; t < totalTime; t += parameters.samplingInterval)
        {
            float currentDistance, currentVelocity;
            
            string phaseType;
            if (t <= accelTime)
            {
                // Acceleration phase
                currentVelocity = parameters.acceleration * t;
                currentDistance = 0.5f * parameters.acceleration * t * t;
                phaseType = "accel";
            }
            else if (t <= accelTime + constantTime)
            {
                // Constant velocity phase
                currentVelocity = parameters.maxVelocity;
                currentDistance = accelDistance + parameters.maxVelocity * (t - accelTime);
                phaseType = "constant";
            }
            else {
                // decel phase
                float decelT = t - accelTime - constantTime;
                currentVelocity = parameters.maxVelocity - parameters.deceleration * decelT;
                currentDistance = accelDistance + constantDistance + 
                                parameters.maxVelocity * decelT - 0.5f * parameters.deceleration * decelT * decelT;
                phaseType = "decel";
            }
            
            Vector3 position = parameters.startPoint + direction * currentDistance;
 
            trajectory.Add(new TrajectoryPoint(position, currentVelocity, t, phaseType));
        }
        
        // Add the exact final point at total time
        trajectory.Add(new TrajectoryPoint(parameters.endPoint, 0f, totalTime, "end"));
        
        return trajectory;
    }
}
