
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class RobotSpatialDashboard : MonoBehaviour
{
    [Header("References")]
    public Camera arCamera;
    public Robot6RARController controller;
    [Header("Telemetry source")]
    public RobotTelemetry telemetry;

    public GameObject dashboard;
    public TMP_Text titleText;
    public TMP_Text telemetryText;
    public Button closeButton;

    [Header("Position")]
    public Vector3 worldOffset =
        new Vector3(0f, 0.12f, 0f);

    public float followSpeed = 12f;

    [Header("Interaction")]
    public LayerMask selectableLayers = ~0;
    public float rayDistance = 100f;

    [Header("Classic UI outside-click dismissal")]
    public RobotMainMenuController mainMenuController;

    private RobotJointSelectable selected;
    private RectTransform dashboardRect;

    void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;

        if (mainMenuController == null)
            mainMenuController = FindAnyObjectByType<RobotMainMenuController>();

        if (telemetry == null)
            telemetry = FindAnyObjectByType<RobotTelemetry>();

        if (dashboard != null)
        {
            dashboardRect =
                dashboard.GetComponent<RectTransform>();

            dashboard.SetActive(false);
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    void Update()
    {
        HandleInput();

        if (selected == null || dashboard == null)
            return;

        UpdatePosition();
        UpdateTelemetry();
    }

    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                TrySelect(touch.position, touch.fingerId);

            return;
        }

        if (Input.GetMouseButtonDown(0))
            TrySelect(Input.mousePosition, -1);
    }

    
void TrySelect(Vector2 screenPosition, int pointerId)
{
    if (arCamera == null)
        return;

    // UI interaction must not select the robot behind it.
    if (EventSystem.current != null &&
        EventSystem.current.IsPointerOverGameObject(pointerId))
        return;

    Ray ray = arCamera.ScreenPointToRay(screenPosition);

    if (Physics.Raycast(
        ray,
        out RaycastHit hit,
        rayDistance,
        selectableLayers,
        QueryTriggerInteraction.Collide))
    {
        RobotJointSelectable joint =
            hit.collider.GetComponentInParent<RobotJointSelectable>();

        if (joint != null)
        {
            Select(joint);
            return;
        }

        RobotBaseSelectable robotBase =
            hit.collider.GetComponentInParent<RobotBaseSelectable>();

        if (robotBase != null)
        {
            Close(); // Close only the individual joint dashboard.
            robotBase.SelectBase();
            return;
        }
    }

    // No automatic dismissal of classic panels.
    // Leave general telemetry and controls open.
    Close(); // Only the individual joint dashboard.
}


    public void Select(RobotJointSelectable target)
    {
        if (target == null)
            return;

        selected = target;

        if (dashboard == null)
            return;

        dashboard.SetActive(true);

        if (dashboardRect == null)
            dashboardRect =
                dashboard.GetComponent<RectTransform>();

        if (dashboardRect != null)
        {
            dashboardRect.position =
                selected.Anchor.position + worldOffset;
        }

        UpdatePosition();
        UpdateTelemetry();
    }

    void UpdatePosition()
{
    if (dashboardRect == null ||
        selected == null ||
        arCamera == null)
        return;

    Vector3 targetPosition =
        selected.Anchor.position + worldOffset;

    float t = 1f -
        Mathf.Exp(-followSpeed * Time.deltaTime);

    dashboardRect.position = Vector3.Lerp(
        dashboardRect.position,
        targetPosition,
        t
    );

    // Point the Canvas front face toward the camera.
    Vector3 direction =
        dashboardRect.position -
        arCamera.transform.position;

    if (direction.sqrMagnitude > 0.000001f)
    {
        dashboardRect.rotation =
            Quaternion.LookRotation(
                direction.normalized,
                arCamera.transform.up
            );
    }
}

    void UpdateTelemetry()
    {
        if (selected == null || controller == null ||
            controller.channels == null)
            return;

        int index = selected.channelIndex;
        if (index < 0 || index >= controller.channels.Length)
            return;

        var channel = controller.channels[index];
        if (channel == null) return;

        if (titleText != null)
            titleText.text = string.IsNullOrEmpty(channel.name)
                ? $"Joint {index + 1}" : channel.name;

        ArticulationBody joint = channel.joint;
        float actual = 0f;
        bool hasMeasurement = joint != null &&
            joint.jointType == ArticulationJointType.RevoluteJoint &&
            joint.jointPosition.dofCount > 0;

        float direction = Mathf.Sign(channel.direction);
        if (hasMeasurement)
        {
            float jointAngle = joint.jointPosition[0] * Mathf.Rad2Deg;
            actual = channel.servoHome + direction *
                (jointAngle - channel.jointHome);
        }

        var text = new System.Text.StringBuilder(256);
        text.AppendLine($"Target: {channel.desired:F1}°");
        text.AppendLine(hasMeasurement ? $"Actual: {actual:F1}°" : "Actual: N/A");
        text.AppendLine(hasMeasurement ? $"Error: {channel.desired - actual:F1}°" : "Error: N/A");

        // The general telemetry and this panel share one sensor source.
        // Match by ArticulationBody to avoid displaying another joint's data
        // if the sensors array has been reordered in the Inspector.
        int sensorIndex = -1;
        RobotTelemetry.JointSensor sensor = null;
        if (telemetry != null && joint != null && telemetry.sensors != null)
        {
            for (int i = 0; i < telemetry.SensorCount; i++)
            {
                if (telemetry.sensors[i] != null &&
                    telemetry.sensors[i].articulation == joint)
                {
                    sensorIndex = i;
                    break;
                }
            }
        }

        if (sensorIndex >= 0 &&
            telemetry.TryGetSensor(sensorIndex, out sensor))
        {
            // The joint velocity is converted to the same servo-angle
            // direction used by Target and Actual.
            float servoVelocity = direction * sensor.velocityDegPerS;
            text.AppendLine($"Velocity: {servoVelocity:F1}°/s");
            text.AppendLine($"Temperature (sim): {telemetry.ReportedTemperatureC(sensorIndex):F1}°C");
            text.AppendLine($"Torque (sim): {sensor.simulatedTorqueNm:F3} N·m");
            text.Append($"Status: {sensor.state}");
        }
        else
        {
            text.AppendLine("Velocity: N/A");
            text.AppendLine("Temperature (sim): N/A");
            text.AppendLine("Torque (sim): N/A");
            text.Append("Status: Unavailable");
        }

        if (telemetryText != null)
            telemetryText.text = text.ToString();
    }

    public void Close()
    {
        selected = null;

        if (dashboard != null)
            dashboard.SetActive(false);
    }
}
