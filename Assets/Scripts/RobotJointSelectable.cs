
using UnityEngine;

public class RobotJointSelectable : MonoBehaviour
{
    [Header("Joint identification")]
    [Range(0, 6)]
    public int channelIndex;

    [Header("References")]
    public ArticulationBody articulation;
    public Transform dashboardAnchor;

    public Transform Anchor =>
        dashboardAnchor != null
            ? dashboardAnchor
            : transform;

    private void Reset()
    {
        articulation =
            GetComponentInParent<ArticulationBody>();
    }
}
