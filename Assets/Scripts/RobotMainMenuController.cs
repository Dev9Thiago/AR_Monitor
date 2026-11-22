
using UnityEngine;
using UnityEngine.UI;

public class RobotMainMenuController : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenu;

    [Header("Existing Interfaces")]
    [SerializeField] private GameObject telemetryInterface;
    [SerializeField] private GameObject controlsInterface;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button telemetryButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button closeButton;

    [Header("Interface Close Buttons")]
    [SerializeField] private Button telemetryCloseButton;
    [SerializeField] private Button controlsCloseButton;

    [Header("Startup")]
    [SerializeField] private bool hideInterfacesOnStart = true;

    private void Awake()
    {
        if (telemetryButton != null)
            telemetryButton.onClick.AddListener(OpenTelemetry);

        if (controlsButton != null)
            controlsButton.onClick.AddListener(OpenControls);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMenu);

        // Close buttons now return to the main menu.
        if (telemetryCloseButton != null)
            telemetryCloseButton.onClick.AddListener(ReturnToMenu);

        if (controlsCloseButton != null)
            controlsCloseButton.onClick.AddListener(ReturnToMenu);

        CloseMenu();

        if (hideInterfacesOnStart)
        {
            CloseTelemetry();
            CloseControls();
        }
    }

    public void OpenMenu()
    {
        CloseTelemetry();
        CloseControls();

        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
    }

    public void OpenTelemetry()
    {
        CloseMenu();
        CloseControls();

        if (telemetryInterface != null)
            telemetryInterface.SetActive(true);
    }

    public void OpenControls()
    {
        CloseMenu();
        CloseTelemetry();

        if (controlsInterface != null)
            controlsInterface.SetActive(true);
    }

    public void CloseTelemetry()
    {
        if (telemetryInterface != null)
            telemetryInterface.SetActive(false);
    }

    public void CloseControls()
    {
        if (controlsInterface != null)
            controlsInterface.SetActive(false);
    }

    public void ReturnToMenu()
    {
        CloseTelemetry();
        CloseControls();

        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    public void CloseAll()
    {
        CloseMenu();
        CloseTelemetry();
        CloseControls();
    }

    private void OnDestroy()
    {
        if (telemetryButton != null)
            telemetryButton.onClick.RemoveListener(OpenTelemetry);

        if (controlsButton != null)
            controlsButton.onClick.RemoveListener(OpenControls);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseMenu);

        if (telemetryCloseButton != null)
            telemetryCloseButton.onClick.RemoveListener(ReturnToMenu);

        if (controlsCloseButton != null)
            controlsCloseButton.onClick.RemoveListener(ReturnToMenu);
    }
}
