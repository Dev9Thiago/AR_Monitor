using UnityEngine;

[DisallowMultipleComponent]
public class RobotSpatialCanvasFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform robotBase;
    [SerializeField] private Camera arCamera;

    [Header("Position relative to robot base (meters)")]
    [SerializeField] private Vector3 localOffset = new Vector3(0.65f, 0.20f, 0f);
    [SerializeField, Min(0f)] private float followSpeed = 12f;

    [Header("Orientation")]
    [SerializeField] private bool faceCamera = true;

    private bool initialized;

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (robotBase == null) return;
        if (arCamera == null) arCamera = Camera.main;

        Vector3 targetPosition = robotBase.TransformPoint(localOffset);
        if (!initialized || followSpeed <= 0f)
        {
            transform.position = targetPosition;
            initialized = true;
        }
        else
        {
            float alpha = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPosition, alpha);
        }

        if (faceCamera && arCamera != null)
        {
            Vector3 awayFromCamera = transform.position - arCamera.transform.position;
            if (awayFromCamera.sqrMagnitude > 0.000001f)
            {
                // World-space Unity UI faces its camera along the canvas's local -Z axis.
                transform.rotation = Quaternion.LookRotation(awayFromCamera.normalized, arCamera.transform.up);
            }
        }
    }

    public void SetRobotBase(Transform target)
    {
        robotBase = target;
        initialized = false;
    }
}
