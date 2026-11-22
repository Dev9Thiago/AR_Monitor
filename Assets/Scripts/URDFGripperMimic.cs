
using UnityEngine;

public class URDFGripperMimic : MonoBehaviour
{
    [Header("Main Driving Gear")]
    public ArticulationBody mainGear;

    [Header("Mimic Articulations")]
    public ArticulationBody leftGear;
    public ArticulationBody leftClaw;
    public ArticulationBody rightGear;
    public ArticulationBody rightClaw;
    public ArticulationBody leftLinkage;
    public ArticulationBody rightLinkage;

    [Header("Main Gear Limits (degrees)")]
    public float minAngle = 0f;
    public float maxAngle = 90f;

    [Header("Mimic Drive Settings")]
    public float stiffness = 10000f;
    public float damping = 100f;
    public float forceLimit = 1000f;

    private void Start()
    {
        Configure(mainGear, 1f);
        Configure(leftGear, -1f);
        Configure(leftClaw, 1f);
        Configure(rightGear, -1f);
        Configure(rightClaw, 1f);
        Configure(leftLinkage, 1f);
        Configure(rightLinkage, 1f);
    }

    private void Configure(
        ArticulationBody joint,
        float multiplier)
    {
        if (joint == null)
            return;

        if (joint.jointType !=
            ArticulationJointType.RevoluteJoint)
        {
            Debug.LogWarning(
                $"{joint.name} is not revolute.", joint);
            return;
        }

        ArticulationDrive drive = joint.xDrive;

        float a = multiplier * minAngle;
        float b = multiplier * maxAngle;

        drive.lowerLimit = Mathf.Min(a, b);
        drive.upperLimit = Mathf.Max(a, b);

        drive.stiffness = stiffness;
        drive.damping = damping;
        drive.forceLimit = forceLimit;

        joint.xDrive = drive;
    }

    private void FixedUpdate()
    {
        if (mainGear == null)
            return;

        // Read the command from Robot6RARController.
        float angle = mainGear.xDrive.target;

        angle = Mathf.Clamp(
            angle, minAngle, maxAngle);

        // Effective URDF mimic multipliers.
        Apply(leftGear, -angle);
        Apply(leftClaw, angle);
        Apply(rightGear, -angle);
        Apply(rightClaw, angle);
        Apply(leftLinkage, angle);
        Apply(rightLinkage, angle);
    }

    private void Apply(
        ArticulationBody joint,
        float target)
    {
        if (joint == null ||
            joint.jointType !=
            ArticulationJointType.RevoluteJoint)
            return;

        ArticulationDrive drive = joint.xDrive;

        drive.target = Mathf.Clamp(
            target,
            drive.lowerLimit,
            drive.upperLimit);

        joint.xDrive = drive;
    }
}
