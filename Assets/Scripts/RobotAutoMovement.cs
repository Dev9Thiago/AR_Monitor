using UnityEngine;
using UnityEngine.UI;

public class RobotAutoMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Robot6RARController robotController;
    [SerializeField] private Button autoMovementButton;
    [SerializeField] private Text buttonLabel;

    [Header("Automatic Movement")]
    [SerializeField, Min(0f)] private float pauseBetweenPositions = 1f;
    [SerializeField, Min(0.01f)] private float arrivalToleranceDegrees = 1f;
    [SerializeField] private bool randomizeGripper = true;

    private bool automaticMode;
    private bool waitingAtTarget;
    private float waitUntil;

    private void Awake()
    {
        if (autoMovementButton != null)
            autoMovementButton.onClick.AddListener(ToggleAutomaticMovement);
        UpdateButtonLabel();
    }

    private void Start()
    {
        // Start after Robot6RARController.Start() has initialized the channels.
        if (robotController == null)
        {
            Debug.LogError("RobotAutoMovement: Assign Robot Controller in the Inspector.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!automaticMode || robotController == null) return;

        if (!AllTargetsReached())
        {
            waitingAtTarget = false;
            return;
        }

        if (!waitingAtTarget)
        {
            waitingAtTarget = true;
            waitUntil = Time.time + pauseBetweenPositions;
        }
        else if (Time.time >= waitUntil)
        {
            GenerateRandomTargets();
            waitingAtTarget = false;
        }
    }

    public void ToggleAutomaticMovement()
    {
        if (automaticMode)
            StopAutomaticMovement();
        else
            StartAutomaticMovement();
    }

    public void StartAutomaticMovement()
    {
        if (robotController == null) return;
        automaticMode = true;
        waitingAtTarget = false;
        GenerateRandomTargets();
        UpdateButtonLabel();
    }

    public void StopAutomaticMovement()
    {
        automaticMode = false;
        waitingAtTarget = false;

        if (robotController != null && robotController.channels != null)
        {
            for (int i = 0; i < robotController.channels.Length; i++)
            {
                var channel = robotController.channels[i];
                if (channel == null) continue;

                // Hold the speed-limited commanded target, not the last random destination.
                float hold = Mathf.Clamp(channel.commanded, channel.minServo, channel.maxServo);
                robotController.SetTarget(i, hold);
                if (channel.slider != null)
                    channel.slider.SetValueWithoutNotify(hold);
            }
        }

        UpdateButtonLabel();
    }

    private void GenerateRandomTargets()
    {
        if (robotController == null || robotController.channels == null) return;

        for (int i = 0; i < robotController.channels.Length; i++)
        {
            var channel = robotController.channels[i];
            if (channel == null) continue;
            if (i == 6 && !randomizeGripper) continue;

            float target = Random.Range(channel.minServo, channel.maxServo);
            robotController.SetTarget(i, target);

            // Keep the existing UI readout and slider consistent with the new target.
            if (channel.slider != null)
                channel.slider.SetValueWithoutNotify(channel.desired);
        }
    }

    private bool AllTargetsReached()
    {
        if (robotController.channels == null) return true;

        for (int i = 0; i < robotController.channels.Length; i++)
        {
            var channel = robotController.channels[i];
            if (channel == null) continue;
            if (Mathf.Abs(channel.commanded - channel.desired) > arrivalToleranceDegrees)
                return false;
        }
        return true;
    }

    private void UpdateButtonLabel()
    {
        if (buttonLabel != null)
            buttonLabel.text = automaticMode ? "STOP" : "AUTO";
    }

    private void OnDestroy()
    {
        if (autoMovementButton != null)
            autoMovementButton.onClick.RemoveListener(ToggleAutomaticMovement);
    }
}
