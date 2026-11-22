using UnityEngine;
using UnityEngine.UI;

/// <summary>Optional diagnostic UI for testing telemetry before World Space dashboards.</summary>
public class RobotTelemetryDebugUI : MonoBehaviour
{
    public RobotTelemetry telemetry;
    public Text output;
    [Min(0.05f)] public float refreshIntervalS = 0.2f;
    private float nextRefresh;

    void Update()
    {
        if (output == null || telemetry == null || Time.unscaledTime < nextRefresh) return;
        nextRefresh = Time.unscaledTime + refreshIntervalS;
        var builder = new System.Text.StringBuilder("ROBOT TELEMETRY\n");
        for (int i = 0; i < telemetry.SensorCount; i++)
        {
            if (!telemetry.TryGetSensor(i, out RobotTelemetry.JointSensor s))
            {
                builder.AppendLine($"J{i + 1}: unavailable");
                continue;
            }
            builder.AppendLine($"{s.name}: {s.positionDeg:F1}° | {s.velocityDegPerS:F1}°/s | " +
                $"T(sim) {telemetry.ReportedTemperatureC(i):F1}°C | " +
                $"τ(sim) {s.simulatedTorqueNm:F3} N·m | {s.state}");
        }
        output.text = builder.ToString();
    }
}
