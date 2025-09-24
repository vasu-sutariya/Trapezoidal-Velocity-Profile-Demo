using UnityEngine;
using UnityEngine.UI;

public class TrajectoryInputHandler : MonoBehaviour
{
    public TrajectoryParams trajectoryParams;
    
    public InputField startX, startY, startZ;
    public InputField endX, endY, endZ;
    public InputField maxVelocity;
    public InputField acceleration;
    public InputField deceleration;
    public InputField samplingInterval;
    
    void Start()
    {
        LoadParamsToFields();
        SetupListeners();
    }
    
    void LoadParamsToFields()
    {
        startX.text = trajectoryParams.startPoint.x.ToString();
        startY.text = trajectoryParams.startPoint.y.ToString();
        startZ.text = trajectoryParams.startPoint.z.ToString();
        endX.text = trajectoryParams.endPoint.x.ToString();
        endY.text = trajectoryParams.endPoint.y.ToString();
        endZ.text = trajectoryParams.endPoint.z.ToString();
        maxVelocity.text = trajectoryParams.maxVelocity.ToString();
        acceleration.text = trajectoryParams.acceleration.ToString();
        deceleration.text = trajectoryParams.deceleration.ToString();
        samplingInterval.text = trajectoryParams.samplingInterval.ToString();
    }
    
    void SetupListeners()
    {
        startX.onEndEdit.AddListener(_ => UpdateStartPoint());
        startY.onEndEdit.AddListener(_ => UpdateStartPoint());
        startZ.onEndEdit.AddListener(_ => UpdateStartPoint());
        endX.onEndEdit.AddListener(_ => UpdateEndPoint());
        endY.onEndEdit.AddListener(_ => UpdateEndPoint());
        endZ.onEndEdit.AddListener(_ => UpdateEndPoint());
        maxVelocity.onEndEdit.AddListener(_ => UpdateMaxVelocity());
        acceleration.onEndEdit.AddListener(_ => UpdateAcceleration());
        deceleration.onEndEdit.AddListener(_ => UpdateDeceleration());
        samplingInterval.onEndEdit.AddListener(_ => UpdateSamplingInterval());
    }
    
    void UpdateStartPoint()
    {
        if (float.TryParse(startX.text, out float x) && 
            float.TryParse(startY.text, out float y) && 
            float.TryParse(startZ.text, out float z))
        {
            trajectoryParams.startPoint = new Vector3(x, y, z);
        }
    }
    
    void UpdateEndPoint()
    {
        if (float.TryParse(endX.text, out float x) && 
            float.TryParse(endY.text, out float y) && 
            float.TryParse(endZ.text, out float z))
        {
            trajectoryParams.endPoint = new Vector3(x, y, z);
        }
    }
    
    void UpdateMaxVelocity()
    {
        if (float.TryParse(maxVelocity.text, out float value))
            trajectoryParams.maxVelocity = value;
    }
    
    void UpdateAcceleration()
    {
        if (float.TryParse(acceleration.text, out float value))
            trajectoryParams.acceleration = value;
    }
    
    void UpdateDeceleration()
    {
        if (float.TryParse(deceleration.text, out float value))
            trajectoryParams.deceleration = value;
    }
    
    void UpdateSamplingInterval()
    {
        if (float.TryParse(samplingInterval.text, out float value))
            trajectoryParams.samplingInterval = value;
    }
}
