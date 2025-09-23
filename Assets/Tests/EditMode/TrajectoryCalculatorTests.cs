using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryCalculatorTests
{
    [TestCase(1f, 5f, 2f, 2f, false, "Triangular profile")]
    [TestCase(100f, 5f, 2f, 2f, true, "Trapezoidal profile")]

    public void GenerateTrajectory_VelocityProfile_WorksCorrectly(
        float distance, float maxVelocity, float acceleration, float deceleration, 
        bool shouldHaveConstantPhase, string profileType)
    {
        var trajectoryParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * distance,
            maxVelocity = maxVelocity,
            acceleration = acceleration,
            deceleration = deceleration,
            samplingInterval = 0.1f
        };

        var trajectory = TrajectoryCalculator.GenerateTrajectory(trajectoryParams);

        Assert.IsNotNull(trajectory);
        Assert.Greater(trajectory.Count, 0);
        Assert.AreEqual(Vector3.zero, trajectory[0].position);
        Assert.AreEqual(Vector3.forward * distance, trajectory[trajectory.Count - 1].position);
        
        // Check velocity profile phases
        bool hasAccel = false, hasConstant = false, hasDecel = false;
        foreach (var point in trajectory)
        {
            if (point.type == "accel") hasAccel = true;
            if (point.type == "constant") hasConstant = true;
            if (point.type == "decel") hasDecel = true;
        }
        
        Assert.IsTrue(hasAccel, $"{profileType} should have acceleration phase");
        Assert.AreEqual(shouldHaveConstantPhase, hasConstant, $"{profileType} constant phase check");
        Assert.IsTrue(hasDecel, $"{profileType} should have deceleration phase");
    }
}
