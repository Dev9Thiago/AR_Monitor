using UnityEngine;

/// <summary>
/// Follows an AR marker with filtered, rate-limited articulation-root teleports.
/// Note: TeleportRoot is still a teleport, not a physics-driven movement.
/// </summary>
public class RobotARTracker : MonoBehaviour
{
    [Header("AR Reference")]
    public Transform cubeAnchor;

    [Header("Robot")]
    public ArticulationBody robotBase;

    [Header("Alignment")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Tracking")]
    public bool followPosition = true;
    public bool followRotation = true;

    [Header("Tracking stability")]
    [Min(0f)] public float positionSmoothing = 12f;
    [Min(0f)] public float rotationSmoothing = 12f;
    [Min(0f)] public float positionDeadbandM = 0.002f;
    [Min(0f)] public float rotationDeadbandDeg = 0.5f;
    [Min(0f)] public float minimumUpdateIntervalS = 0.04f;

    private Vector3 filteredPosition;
    private Quaternion filteredRotation;
    private bool filterInitialized;
    private float elapsedSinceTeleport;
    private bool previousFollowPosition;
    private bool previousFollowRotation;

    private void OnEnable()
    {
        filterInitialized = false;
        elapsedSinceTeleport = 0f;
        previousFollowPosition = followPosition;
        previousFollowRotation = followRotation;
    }

    private void FixedUpdate()
    {
        if (cubeAnchor == null || robotBase == null)
        {
            filterInitialized = false;
            return;
        }

        // Never teleport when neither axis of tracking is enabled.
        if (!followPosition && !followRotation)
        {
            filterInitialized = false;
            previousFollowPosition = false;
            previousFollowRotation = false;
            return;
        }

        Vector3 desiredPosition = cubeAnchor.TransformPoint(positionOffset);
        Quaternion desiredRotation = cubeAnchor.rotation * Quaternion.Euler(rotationOffset);

        bool modeChanged = followPosition != previousFollowPosition ||
                           followRotation != previousFollowRotation;
        previousFollowPosition = followPosition;
        previousFollowRotation = followRotation;

        if (!filterInitialized || modeChanged)
        {
            // Initialize at the robot's current pose so enabling tracking
            // does not immediately cause a full, unfiltered jump.
            filteredPosition = robotBase.transform.position;
            filteredRotation = robotBase.transform.rotation;
            filterInitialized = true;
        }

        float dt = Time.fixedDeltaTime;
        if (followPosition)
        {
            float alpha = positionSmoothing <= 0f ? 1f :
                1f - Mathf.Exp(-positionSmoothing * dt);
            filteredPosition = Vector3.Lerp(filteredPosition, desiredPosition, alpha);
        }
        else
        {
            filteredPosition = robotBase.transform.position;
        }

        if (followRotation)
        {
            float alpha = rotationSmoothing <= 0f ? 1f :
                1f - Mathf.Exp(-rotationSmoothing * dt);
            filteredRotation = Quaternion.Slerp(filteredRotation, desiredRotation, alpha);
        }
        else
        {
            filteredRotation = robotBase.transform.rotation;
        }

        elapsedSinceTeleport += dt;
        if (elapsedSinceTeleport < minimumUpdateIntervalS)
            return;

        Vector3 currentPosition = robotBase.transform.position;
        Quaternion currentRotation = robotBase.transform.rotation;
        Vector3 targetPosition = followPosition ? filteredPosition : currentPosition;
        Quaternion targetRotation = followRotation ? filteredRotation : currentRotation;

        bool positionChanged = followPosition &&
            Vector3.Distance(currentPosition, targetPosition) >= positionDeadbandM;
        bool rotationChanged = followRotation &&
            Quaternion.Angle(currentRotation, targetRotation) >= rotationDeadbandDeg;

        if (!positionChanged && !rotationChanged)
            return;

        robotBase.TeleportRoot(targetPosition, targetRotation);
        elapsedSinceTeleport = 0f;
    }
}
