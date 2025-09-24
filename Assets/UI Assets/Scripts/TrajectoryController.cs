using UnityEngine;

public class TrajectoryController : MonoBehaviour
{
    public TrajectoryParams trajectoryParams;
    public UnityTrajectory unityTrajectory;
    public TrajectoryInputHandler inputHandler;
    
    void Start()
    {
        inputHandler.trajectoryParams = trajectoryParams;
        unityTrajectory.trajectoryParams = trajectoryParams;
    }
    
    public void StartTrajectory()
    {
        unityTrajectory.StartTrajectory();
    }
}
