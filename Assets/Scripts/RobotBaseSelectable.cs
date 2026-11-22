using UnityEngine;

public class RobotBaseSelectable : MonoBehaviour
{
    [SerializeField] private RobotMainMenuController menuController;

    public void SelectBase()
    {
        if (menuController != null)
            menuController.OpenMenu();
        else
            Debug.LogWarning("RobotBaseSelectable: assign Menu Controller in the Inspector.", this);
    }
}
