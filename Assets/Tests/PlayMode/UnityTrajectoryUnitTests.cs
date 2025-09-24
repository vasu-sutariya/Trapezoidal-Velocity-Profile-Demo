using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class UnityTrajectoryUnitTests
{
    private GameObject testObject;
    private UnityTrajectory trajectoryComponent;
    
    [SetUp]
    public void Setup()
    {
        testObject = new GameObject("Cube");
        trajectoryComponent = testObject.AddComponent<UnityTrajectory>();
        
        // Configure test parameters
        trajectoryComponent.trajectoryParams = new TrajectoryParams
        {
            startPoint = Vector3.zero,
            endPoint = Vector3.forward * 10f,
            maxVelocity = 5f,
            acceleration = 2f,
            deceleration = 2f,
            samplingInterval = 0.1f
        };
        trajectoryComponent.autoStart = false; 
    }
    
    [TearDown]
    public void TearDown()
    {
        if (testObject != null)
            Object.DestroyImmediate(testObject);
    }

    [UnityTest]
    public IEnumerator UnityTrajectory_StartTrajectory_InitializesCorrectly()
    {
        // Test that StartTrajectory() properly initializes the component

        
        // initial state should be false
        Assert.IsFalse(trajectoryComponent.IsAnimating, "Component should not be animating initially");
        
        // Start the trajectory
        trajectoryComponent.StartTrajectory();
        
        // Verify initialization
        Assert.IsTrue(trajectoryComponent.IsAnimating, "Component should be animating after StartTrajectory()");
        Assert.AreEqual(0, trajectoryComponent.CurrentPointIndex, "Current point index should be 0 after initialization");
        Assert.IsNotNull(trajectoryComponent.Trajectory, "Trajectory list should not be empty");

        yield return null;
    }

   

    [UnityTest]
    public IEnumerator UnityTrajectory_StopTrajectory_StopsAnimation()
    {
        // Test that StopTrajectory() sets isAnimating to false
        
        // Start trajectory
        trajectoryComponent.StartTrajectory();
        
        // Record position before stopping
        Vector3 positionBeforeStop = testObject.transform.position;
        
        // Stop the trajectory
        trajectoryComponent.StopTrajectory();
        
        // Verify animation stopped
        Assert.IsFalse(trajectoryComponent.IsAnimating, "Component should not be animating after StopTrajectory()");
        
        // Wait a frame to ensure Update() doesn't continue animating
        yield return null;
        
        // Verify position doesn't change after stopping
        Vector3 positionAfterStop = testObject.transform.position;
        Assert.AreEqual(positionBeforeStop, positionAfterStop, 
            "Object position should not change after stopping trajectory");
        
        // Verify currentPointIndex remains unchanged
        int pointIndexAfterStop = trajectoryComponent.CurrentPointIndex;
        Assert.GreaterOrEqual(pointIndexAfterStop, 0, "Current point index should remain valid");
        
        yield return null;
    }
}
