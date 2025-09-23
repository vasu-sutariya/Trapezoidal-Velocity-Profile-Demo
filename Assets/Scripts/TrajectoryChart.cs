using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

namespace TrajectoryDemo
{
    // Visualize trajectory parameters using XCharts
    public class TrajectoryChart : MonoBehaviour
    {
        [Header("Chart References")]
        public LineChart chart;
        public UnityTrajectory trajectorySource;
        
        void Start()
        {
           CreateChart();
        }
        
        public void CreateChart()
        {
            if (chart == null || trajectorySource == null) return;
            
            // Clear existing  data
            chart.ClearData();
            
            
            
            // Add legend
            var legend = chart.EnsureChartComponent<Legend>();
            legend.show = true;
            legend.location.align = Location.Align.TopRight;
            
            // Add series 
            chart.AddSerie<Line>("Velocity");
            chart.AddSerie<Line>("Distance");
            chart.AddSerie<Line>("Acceleration");
            
            var trajectory = TrajectoryCalculator.GenerateTrajectory(trajectorySource.trajectoryParams);
            Vector3 startPos = trajectorySource.trajectoryParams.startPoint;
            
            foreach (var point in trajectory)
            {
                float distance = Vector3.Distance(startPos, point.position);
                
                float acceleration = 0f;
                if (point.type == "accel")
                    acceleration = trajectorySource.trajectoryParams.acceleration;
                else if (point.type == "decel")
                    acceleration = -trajectorySource.trajectoryParams.deceleration;
                
                chart.AddData(0, point.time, point.velocity);
                chart.AddData(1, point.time, distance);
                chart.AddData(2, point.time, acceleration);
            }
        }
        
    }
}
