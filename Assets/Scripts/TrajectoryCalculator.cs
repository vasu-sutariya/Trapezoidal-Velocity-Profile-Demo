using System.Collections.Generic;
using UnityEngine;

public static class TrajectoryCalculator
{
    public static List<TrajectoryPoint> GenerateTrajectory(TrajectoryParams parameters)
    {
        List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();

        // Validate input parameters
        if (!ValidateParameters(parameters))
        {
            return trajectory;
        }

        float distance = Vector3.Distance(parameters.startPoint, parameters.endPoint);
        if (distance <= 1e-6f) {
            // already at target
            trajectory.Add(new TrajectoryPoint(parameters.startPoint, 0f, 0f, "end"));
            return trajectory;
        }

        Vector3 direction = (parameters.endPoint - parameters.startPoint).normalized;

        // Calculate phase durations and distances
        float accelTime = parameters.maxVelocity / parameters.acceleration;
        float decelTime = parameters.maxVelocity / parameters.deceleration;
        float accelDistance = 0.5f * parameters.acceleration * accelTime * accelTime;
        float decelDistance = 0.5f * parameters.deceleration * decelTime * decelTime;

        float peakVelocity = parameters.maxVelocity;

        // Check triangular vs trapezoidal
        if (accelDistance + decelDistance > distance)
        {
            // Triangular profile: reassign peak velocity and times
            peakVelocity = Mathf.Sqrt(
                2f * distance * parameters.acceleration * parameters.deceleration
                / (parameters.acceleration + parameters.deceleration));

            accelTime = peakVelocity / parameters.acceleration;
            decelTime = peakVelocity / parameters.deceleration;

            accelDistance = 0.5f * parameters.acceleration * accelTime * accelTime;
            decelDistance = distance - accelDistance;  
        }

        float constantDistance = Mathf.Max(0f, distance - accelDistance - decelDistance);
        float constantTime = (peakVelocity > 0f) ? (constantDistance / peakVelocity) : 0f;
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
            
            else if (t <= accelTime + constantTime )
            {
                // Constant-velocity phase 
                float tc = t - accelTime;
                currentVelocity = peakVelocity; 
                currentDistance = accelDistance + peakVelocity * tc; 
                phaseType = "constant";
            }
            else
            {
                // decel phase
                float td = t - accelTime - constantTime;
                currentVelocity = Mathf.Max(0f, peakVelocity - parameters.deceleration * td); 
                currentDistance = accelDistance + constantDistance
                                  + peakVelocity * td
                                  - 0.5f * parameters.deceleration * td * td; 
                phaseType = "decel";
            }

            
            Vector3 position = parameters.startPoint + direction * currentDistance;
            trajectory.Add(new TrajectoryPoint(position, currentVelocity, t, phaseType));
        }

        // Add the exact final point at total time
        trajectory.Add(new TrajectoryPoint(parameters.endPoint, 0f, totalTime, "end"));
        return trajectory;
    }


    // Validates trajectory parameters and logs appropriate error messages for invalid values.

    private static bool ValidateParameters(TrajectoryParams parameters)
    {
        // Check for null parameters
        if (parameters == null)
        {
            Debug.LogError("TrajectoryCalculator: Parameters cannot be null");
            return false;
        }

        // Validate acceleration
        if (parameters.acceleration <= 0f)
        {
            Debug.LogError($"TrajectoryCalculator: Invalid acceleration value: {parameters.acceleration}. Must be greater than 0.");
            return false;
        }
        
        // Validate deceleration
        if (parameters.deceleration <= 0f)
        {
            Debug.LogError($"TrajectoryCalculator: Invalid deceleration value: {parameters.deceleration}. Must be greater than 0.");
            return false;
        }
        
        // Validate sampling interval
        if (parameters.samplingInterval <= 0f)
        {
            Debug.LogError($"TrajectoryCalculator: Invalid sampling interval: {parameters.samplingInterval}. Must be greater than 0.");
            return false;
        }

        // Validate max velocity
        if (parameters.maxVelocity <= 0f)
        {
            Debug.LogError($"TrajectoryCalculator: Invalid max velocity: {parameters.maxVelocity}. Must be greater than 0.");
            return false;
        }

        return true;
    }
}
