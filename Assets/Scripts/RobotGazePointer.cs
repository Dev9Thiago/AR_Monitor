using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Camera-centered gaze interaction for robot colliders and World Space UI.
/// Robot selection: 2 s. UI buttons/sliders: 5 s.
/// Captured sliders respond to horizontal camera rotation and are released
/// when the gaze reaches a UI panel background or another UI element.
/// </summary>
public class RobotGazePointer : MonoBehaviour
{
    [Header("References")]
    public Camera arCamera;
    public RobotSpatialDashboard spatialDashboard;
    public EventSystem eventSystem;

    [Header("Robot gaze")]
    [Min(0.1f)] public float dwellSeconds = 2f;
    [Min(0.01f)] public float maxDistance = 100f;
    public LayerMask robotLayers = ~0;
    public bool enableGaze = true;

    [Header("World Space UI gaze")]
    [Min(0.1f)] public float uiDwellSeconds = 5f;
    [Tooltip("Degrees of camera yaw required to traverse a slider's entire range.")]
    [Min(1f)] public float sliderYawRangeDegrees = 50f;
    [Tooltip("If true, camera rotation to the right increases the slider value.")]
    public bool rightIncreasesValue = true;

    [Header("Reticle")]
    public bool showReticle = true;
    public int reticleSizePixels = 24;

    private Object hovered;
    private Object activated;
    private float dwellStart;
    private float progress;
    private Slider capturedSlider;
    private Vector3 previousCameraForward;
    private Texture2D pixel;
    private PointerEventData pointerData;
    private readonly List<RaycastResult> uiHits = new List<RaycastResult>();

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (spatialDashboard == null)
            spatialDashboard = FindAnyObjectByType<RobotSpatialDashboard>();
        if (eventSystem == null) eventSystem = EventSystem.current;
        pixel = new Texture2D(1, 1);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();
    }

    private void OnDestroy()
    {
        if (pixel != null) Destroy(pixel);
    }

    private void Update()
    {
        if (!enableGaze || arCamera == null)
        {
            ReleaseSlider();
            ResetHover();
            return;
        }

        if (eventSystem == null) eventSystem = EventSystem.current;
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        GameObject uiHit = GetFirstUIHit(center);

        if (capturedSlider != null)
        {
            if (!capturedSlider.gameObject.activeInHierarchy || !capturedSlider.IsInteractable())
            {
                ReleaseSlider();
                ResetHover();
                return;
            }

            // Any other visible UI graphic, including the panel background,
            // is an intentional release zone. Looking away from all UI
            // does not immediately release, allowing camera adjustment.
            if (uiHit != null && !IsPartOfSlider(uiHit, capturedSlider))
            {
                ReleaseSlider();
                ResetHover();
                return;
            }

            AdjustCapturedSlider();
            progress = 1f;
            return;
        }

        Object target = null;
        float requiredSeconds = dwellSeconds;

        // UI has priority: never select the robot through a visible UI graphic.
        if (uiHit != null)
        {
            Button button = uiHit.GetComponentInParent<Button>();
            Slider slider = uiHit.GetComponentInParent<Slider>();
            if (button != null && button.IsActive() && button.IsInteractable())
                target = button;
            else if (slider != null && slider.IsActive() && slider.IsInteractable())
                target = slider;
            requiredSeconds = uiDwellSeconds;
        }
        else if (spatialDashboard != null)
        {
            Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance, robotLayers,
                QueryTriggerInteraction.Collide);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in hits)
            {
                RobotJointSelectable joint = hit.collider.GetComponentInParent<RobotJointSelectable>();
                if (joint != null) { target = joint; break; }
                RobotBaseSelectable robotBase = hit.collider.GetComponentInParent<RobotBaseSelectable>();
                if (robotBase != null) { target = robotBase; break; }
            }
        }

        if (target != hovered)
        {
            hovered = target;
            activated = null;
            dwellStart = Time.unscaledTime;
            progress = 0f;
        }
        if (hovered == null) { progress = 0f; return; }
        if (hovered == activated) { progress = 1f; return; }

        progress = Mathf.Clamp01((Time.unscaledTime - dwellStart) /
            Mathf.Max(0.1f, requiredSeconds));
        if (progress < 1f) return;

        activated = hovered;
        if (hovered is Button selectedButton)
        {
            // Unity Button.onClick is the same callback used by mouse/touch.
            selectedButton.onClick.Invoke();
        }
        else if (hovered is Slider selectedSlider)
        {
            capturedSlider = selectedSlider;
            previousCameraForward = arCamera.transform.forward;
        }
        else if (hovered is RobotJointSelectable selectedJoint)
        {
            spatialDashboard.Select(selectedJoint);
        }
        else if (hovered is RobotBaseSelectable selectedBase)
        {
            spatialDashboard.Close();
            selectedBase.SelectBase();
        }
    }

    private GameObject GetFirstUIHit(Vector2 center)
    {
        if (eventSystem == null) return null;
        if (pointerData == null) pointerData = new PointerEventData(eventSystem);
        pointerData.Reset();
        pointerData.position = center;
        uiHits.Clear();
        eventSystem.RaycastAll(pointerData, uiHits);
        foreach (RaycastResult hit in uiHits)
        {
            if (hit.gameObject != null && hit.gameObject.activeInHierarchy)
                return hit.gameObject;
        }
        return null;
    }

    private static bool IsPartOfSlider(GameObject hit, Slider slider)
    {
        return hit != null && slider != null &&
            hit.transform.IsChildOf(slider.transform);
    }

    private void AdjustCapturedSlider()
    {
        Vector3 currentForward = arCamera.transform.forward;
        Vector3 previousFlat = Vector3.ProjectOnPlane(previousCameraForward, Vector3.up);
        Vector3 currentFlat = Vector3.ProjectOnPlane(currentForward, Vector3.up);
        previousCameraForward = currentForward;
        if (previousFlat.sqrMagnitude < 0.0001f || currentFlat.sqrMagnitude < 0.0001f)
            return;

        float deltaYaw = Vector3.SignedAngle(previousFlat, currentFlat, Vector3.up);
        float direction = rightIncreasesValue ? 1f : -1f;
        float range = capturedSlider.maxValue - capturedSlider.minValue;
        capturedSlider.value += direction * deltaYaw / sliderYawRangeDegrees * range;
    }

    private void ReleaseSlider()
    {
        capturedSlider = null;
    }

    private void ResetHover()
    {
        hovered = null;
        activated = null;
        progress = 0f;
    }

    private void OnGUI()
    {
        if (!enableGaze || !showReticle || pixel == null) return;
        float x = Screen.width * 0.5f;
        float y = Screen.height * 0.5f;
        float s = Mathf.Max(12, reticleSizePixels);
        Color previous = GUI.color;
        GUI.color = capturedSlider != null ? Color.yellow :
            hovered == null ? Color.white : (progress >= 1f ? Color.green : Color.cyan);
        GUI.DrawTexture(new Rect(x - s / 2, y - 1, s, 2), pixel);
        GUI.DrawTexture(new Rect(x - 1, y - s / 2, 2, s), pixel);
        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(x - 23, y + s / 2 + 7, 46, 5), pixel);
        GUI.color = capturedSlider != null ? Color.yellow : Color.cyan;
        GUI.DrawTexture(new Rect(x - 22, y + s / 2 + 8, 44 * progress, 3), pixel);
        GUI.color = previous;
    }
}
