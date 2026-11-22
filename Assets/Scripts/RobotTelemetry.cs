using System;
using UnityEngine;

/// <summary>
/// Read-only joint telemetry plus explicitly simulated motor diagnostics.
/// Does not command drives, modify transforms, or interact with Vuforia.
/// </summary>
public class RobotTelemetry : MonoBehaviour
{
    public enum JointMotionState { Unavailable, Stationary, Moving, NearLimit }

    [Serializable]
    public class JointSensor
    {
        public string name = "Joint";
        public ArticulationBody articulation;
        [Tooltip("True for a physical revolute joint; false for non-articulated entries.")]
        public bool enabled = true;
        [Tooltip("Approximate ambient motor temperature (°C).")]
        public float ambientTemperatureC = 24f;
        [Tooltip("Temperature increase per degree/second at steady state (°C per °/s).")]
        public float temperaturePerSpeed = 0.07f;
        [Tooltip("Time constant of the temperature model (seconds).")]
        public float thermalTimeConstantS = 12f;
        [Tooltip("Synthetic load torque at zero velocity (N·m).")]
        public float baselineTorqueNm = 0.03f;
        [Tooltip("Additional synthetic torque per degree/second (N·m per °/s).")]
        public float torquePerSpeed = 0.002f;

        [NonSerialized] public bool valid;
        [NonSerialized] public float positionDeg;
        [NonSerialized] public float velocityDegPerS;
        [NonSerialized] public float rawVelocityDegPerS;
        [NonSerialized] public float temperatureC;
        [NonSerialized] public float simulatedTorqueNm;
        [NonSerialized] public JointMotionState state;
        [NonSerialized] public bool initialized;
        [NonSerialized] public float previousPositionDeg;
        [NonSerialized] public float elapsedS;
    }

    [Header("Joint sensors (assign each revolute ArticulationBody)")]
    public JointSensor[] sensors = new JointSensor[7];

    [Header("Status thresholds")]
    [Min(0f)] public float movingThresholdDegPerS = 0.5f;
    [Min(0f)] public float limitMarginDeg = 2f;

    [Header("Velocity reporting")]
    [Tooltip("Time constant of the displayed joint velocity (seconds); 0 disables filtering.")]
    [Min(0f)] public float velocitySmoothingTimeS = 0.1f;

    [Header("Synthetic signal")]
    [Min(0f)] public float temperatureRippleC = 0.2f;
    [Min(0f)] public float torqueRippleNm = 0.005f;

    public int SensorCount => sensors == null ? 0 : sensors.Length;

    void FixedUpdate()
    {
        if (sensors == null) return;
        float dt = Time.fixedDeltaTime;
        if (dt <= 0f) return;

        for (int i = 0; i < sensors.Length; i++)
        {
            JointSensor s = sensors[i];
            if (s == null) continue;

            bool available = s.enabled && s.articulation != null &&
                s.articulation.jointType == ArticulationJointType.RevoluteJoint &&
                s.articulation.jointPosition.dofCount > 0;

            if (!available)
            {
                s.valid = false;
                s.initialized = false;
                s.state = JointMotionState.Unavailable;
                continue;
            }

            s.valid = true;
            float angle = s.articulation.jointPosition[0] * Mathf.Rad2Deg;
            s.positionDeg = angle;

            if (!s.initialized)
            {
                s.previousPositionDeg = angle;
                s.velocityDegPerS = 0f;
                s.rawVelocityDegPerS = 0f;
                s.temperatureC = s.ambientTemperatureC;
                s.elapsedS = 0f;
                s.initialized = true;
            }
            else
            {
                // Read the reduced-coordinate velocity directly from the physics solver.
                // This is relative joint motion, not world-space link motion.
                var jointVelocity = s.articulation.jointVelocity;
                s.rawVelocityDegPerS = jointVelocity.dofCount > 0
                    ? jointVelocity[0] * Mathf.Rad2Deg
                    : 0f;
                float velocityAlpha = velocitySmoothingTimeS <= 0f
                    ? 1f
                    : 1f - Mathf.Exp(-dt / velocitySmoothingTimeS);
                s.velocityDegPerS = Mathf.Lerp(
                    s.velocityDegPerS, s.rawVelocityDegPerS, velocityAlpha);
                s.previousPositionDeg = angle;
            }

            s.elapsedS += dt;
            // Diagnostics are simulated from the filtered velocity.
            float speed = Mathf.Abs(s.velocityDegPerS);
            float thermalTarget = s.ambientTemperatureC + speed * Mathf.Max(0f, s.temperaturePerSpeed);
            float tau = Mathf.Max(0.01f, s.thermalTimeConstantS);
            float alpha = 1f - Mathf.Exp(-dt / tau);
            s.temperatureC = Mathf.Lerp(s.temperatureC, thermalTarget, alpha);

            // Deterministic synthetic fluctuations, independent of frame rate.
            float phase = i * 1.7f;
            float torqueNoise = torqueRippleNm * Mathf.Sin(s.elapsedS * 2.3f + phase);
            s.simulatedTorqueNm = Mathf.Max(0f,
                s.baselineTorqueNm + speed * Mathf.Max(0f, s.torquePerSpeed) + torqueNoise);

            ArticulationDrive drive = s.articulation.xDrive;
            bool hasLimits = drive.lowerLimit < drive.upperLimit;
            bool nearLimit = hasLimits &&
                (angle <= drive.lowerLimit + limitMarginDeg ||
                 angle >= drive.upperLimit - limitMarginDeg);

            s.state = nearLimit ? JointMotionState.NearLimit :
                speed >= movingThresholdDegPerS ? JointMotionState.Moving :
                JointMotionState.Stationary;

            // Add a ripple only to the reported temperature, without contaminating
            // the thermal state integrated above. Consumers may use ReportedTemperatureC.
        }
    }

    public bool TryGetSensor(int index, out JointSensor sensor)
    {
        sensor = null;
        if (sensors == null || index < 0 || index >= sensors.Length) return false;
        sensor = sensors[index];
        return sensor != null && sensor.valid;
    }

    public float ReportedTemperatureC(int index)
    {
        if (!TryGetSensor(index, out JointSensor s)) return float.NaN;
        return s.temperatureC + temperatureRippleC * Mathf.Sin(s.elapsedS * 1.1f + index * 1.7f);
    }
}
