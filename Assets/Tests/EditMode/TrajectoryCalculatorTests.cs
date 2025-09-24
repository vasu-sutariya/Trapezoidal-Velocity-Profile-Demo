using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;


public class TrajectoryCalculatorTests
{
    [TestCase(1f, 5f, 2f, 2f, false, "Triangular profile")]
    [TestCase(100f, 5f, 2f, 2f, true, "Trapezoidal profile")]
    public void GenerateTrajectory_ProfileShape_SeparateTriangularFromTrapezoidal(
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

        // Check velocity profile phases
        bool hasConstant = false;
        foreach (var point in trajectory)
        {
            if (point.type == "constant") hasConstant = true;
        }
        
        Assert.AreEqual(shouldHaveConstantPhase, hasConstant, $"{profileType} constant phase check");
    }

    [TestCase(1f, 5f, 2f, 2f, false, "Triangular profile")]
    [TestCase(100f, 5f, 2f, 2f, true, "Trapezoidal profile")]
    public void GenerateTrajectory_EdgePoints_ClosesToTarget(float distance, float maxVelocity, float acceleration, float deceleration, 
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
        Assert.That(trajectory[trajectory.Count - 1].position, Is.EqualTo(Vector3.forward * distance), $"{profileType} profile should close to target");
        
    
    }

    [TestCase(1f, 5f, 2f, 2f, false, "Triangular profile")]
    [TestCase(100f, 5f, 2f, 2f, true, "Trapezoidal profile")]
    public void GenerateTrajectory_EdgePoints_StartsAtOrigin(float distance, float maxVelocity, float acceleration, float deceleration, 
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
        Assert.That(trajectory[0].position, Is.EqualTo(Vector3.zero), $"{profileType} profile should start at origin");        
    
    }

    [TestCase(1f, 5f, 2f, 2f, false, "Triangular profile")]
    [TestCase(100f, 5f, 2f, 2f, true, "Trapezoidal profile")]
    public void GenerateTrajectory_Continuity_DistanceIncreases(float distance, float maxVelocity, float acceleration, float deceleration, 
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
        for (int i = 1; i < trajectory.Count; i++)
        {
            float currentDistance = Vector3.Distance(trajectoryParams.startPoint, trajectory[i].position);
            float previousDistance = Vector3.Distance(trajectoryParams.startPoint, trajectory[i - 1].position);
            Assert.GreaterOrEqual(currentDistance, previousDistance, $"{profileType} profile should have increasing distance");
        }
    
    }

    [TestCase(1f, 5f, 2f, 2f, false, "Triangular profile - equal accel/decel")]
    [TestCase(100f, 5f, 2f, 2f, true, "Trapezoidal profile - equal accel/decel")]
    [TestCase(1f, 5f, 3f, 1f, false, "Triangular profile - asymmetric accel/decel")]
    [TestCase(100f, 5f, 3f, 1f, true, "Trapezoidal profile - asymmetric accel/decel")]
    public void GenerateTrajectory_VelocityProfile_ReachesPeakVelocity(float distance, float maxVelocity, float acceleration, float deceleration, 
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
        float vmax = 0f; 
        foreach (var point in trajectory) vmax = Mathf.Max(vmax, point.velocity);

        if (!shouldHaveConstantPhase)
        {
            float vp = Mathf.Sqrt(2f * distance * acceleration * deceleration
                / (acceleration + deceleration));
            Assert.That(vmax, Is.EqualTo(vp).Within(0.1f), $"{profileType} profile should reach peak velocity");
            Assert.Less(vmax, maxVelocity, $"{profileType} profile should reach peak velocity");
        }
        else
        {
            Assert.That(vmax, Is.EqualTo(maxVelocity).Within(1e-2f), $"{profileType} profile should reach peak velocity");
        }
    
    }

    [Test]
    public void ValidateParameters_InvalidParameters_LogsDebugErrors()
    {
        // Test null parameters 
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Parameters cannot be null");
        var nullTrajectory = TrajectoryCalculator.GenerateTrajectory(null);
        Assert.That(nullTrajectory.Count, Is.EqualTo(0), "Null parameters should return empty trajectory");

        // Test negative acceleration
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid acceleration value: -2. Must be greater than 0.");
        var negativeAccelParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 5f,
            acceleration = -2f, // Invalid negative acceleration
            deceleration = 2f,
            samplingInterval = 0.1f
        };
        var negativeAccelTrajectory = TrajectoryCalculator.GenerateTrajectory(negativeAccelParams);
        Assert.That(negativeAccelTrajectory.Count, Is.EqualTo(0), "Negative acceleration should return empty trajectory");

        // Test zero acceleration
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid acceleration value: 0. Must be greater than 0.");
        var zeroAccelParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 5f,
            acceleration = 0f, // Invalid zero acceleration
            deceleration = 2f,
            samplingInterval = 0.1f
        };
        var zeroAccelTrajectory = TrajectoryCalculator.GenerateTrajectory(zeroAccelParams);
        Assert.That(zeroAccelTrajectory.Count, Is.EqualTo(0), "Zero acceleration should return empty trajectory");

        // Test negative deceleration
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid deceleration value: -1. Must be greater than 0.");
        var negativeDecelParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 5f,
            acceleration = 2f,
            deceleration = -1f, // Invalid negative deceleration
            samplingInterval = 0.1f
        };
        var negativeDecelTrajectory = TrajectoryCalculator.GenerateTrajectory(negativeDecelParams);
        Assert.That(negativeDecelTrajectory.Count, Is.EqualTo(0), "Negative deceleration should return empty trajectory");

        // Test zero deceleration
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid deceleration value: 0. Must be greater than 0.");
        var zeroDecelParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 5f,
            acceleration = 2f,
            deceleration = 0f, // Invalid zero deceleration
            samplingInterval = 0.1f
        };
        var zeroDecelTrajectory = TrajectoryCalculator.GenerateTrajectory(zeroDecelParams);
        Assert.That(zeroDecelTrajectory.Count, Is.EqualTo(0), "Zero deceleration should return empty trajectory");

        // Test negative sampling interval
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid sampling interval: -0.1. Must be greater than 0.");
        var negativeSamplingParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 5f,
            acceleration = 2f,
            deceleration = 2f,
            samplingInterval = -0.1f // Invalid negative sampling interval
        };
        var negativeSamplingTrajectory = TrajectoryCalculator.GenerateTrajectory(negativeSamplingParams);
        Assert.That(negativeSamplingTrajectory.Count, Is.EqualTo(0), "Negative sampling interval should return empty trajectory");

        // Test zero sampling interval
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid sampling interval: 0. Must be greater than 0.");
        var zeroSamplingParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 5f,
            acceleration = 2f,
            deceleration = 2f,
            samplingInterval = 0f // Invalid zero sampling interval
        };
        var zeroSamplingTrajectory = TrajectoryCalculator.GenerateTrajectory(zeroSamplingParams);
        Assert.That(zeroSamplingTrajectory.Count, Is.EqualTo(0), "Zero sampling interval should return empty trajectory");

        // Test negative max velocity
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid max velocity: -5. Must be greater than 0.");
        var negativeMaxVelParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = -5f, // Invalid negative max velocity
            acceleration = 2f,
            deceleration = 2f,
            samplingInterval = 0.1f
        };
        var negativeMaxVelTrajectory = TrajectoryCalculator.GenerateTrajectory(negativeMaxVelParams);
        Assert.That(negativeMaxVelTrajectory.Count, Is.EqualTo(0), "Negative max velocity should return empty trajectory");

        // Test zero max velocity
        LogAssert.Expect(LogType.Error, "TrajectoryCalculator: Invalid max velocity: 0. Must be greater than 0.");
        var zeroMaxVelParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 0f, // Invalid zero max velocity
            acceleration = 2f,
            deceleration = 2f,
            samplingInterval = 0.1f
        };
        var zeroMaxVelTrajectory = TrajectoryCalculator.GenerateTrajectory(zeroMaxVelParams);
        Assert.That(zeroMaxVelTrajectory.Count, Is.EqualTo(0), "Zero max velocity should return empty trajectory");
    }

}
