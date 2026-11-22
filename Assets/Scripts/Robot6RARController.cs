
using System;
using UnityEngine;
using UnityEngine.UI;

public class Robot6RARController : MonoBehaviour
{
    [Serializable]
    public class JointChannel
    {
        public string name;
        public ArticulationBody joint;
        public Slider slider;
        public Text readout;

        public float minServo = 0f;
        public float maxServo = 180f;
        public float servoHome = 0f;

        public float jointHome = 0f;
        public float direction = 1f;
        public float maxSpeed = 300f;

        [NonSerialized] public float desired;
        [NonSerialized] public float commanded;
    }

    public JointChannel[] channels =
        new JointChannel[7];

    public Button homeButton;
    public Transform baseFrame;
    public Transform tcpFrame;
    public Text tcpReadout;

    private void Reset()
    {
        string[] names = {
            "q1 - DS3218MG",
            "q2 - DS3218MG",
            "q3 - DS3218MG",
            "q4 - MG995R",
            "q5 - DS3218MG",
            "q6 - MG90S",
            "Gripper - MG90S"
        };

        float[] homes = {
            0f, 0f, 90f, 0f, 90f, 0f, 0f
        };

        channels = new JointChannel[7];

        for (int i = 0; i < channels.Length; i++)
        {
            channels[i] = new JointChannel();
            channels[i].name = names[i];
            channels[i].servoHome = homes[i];

            channels[i].maxServo =
                i == 6 ? 90f : 180f;
        }
    }

    private void Start()
    {
        for (int i = 0; i < channels.Length; i++)
        {
            int index = i;
            JointChannel c = channels[i];

            if (c == null)
                continue;

            // Gripper range is 0 to 90 degrees.
            if (i == 6)
            {
                c.minServo = 0f;
                c.maxServo = 90f;
            }

            c.desired = c.servoHome;
            c.commanded = c.servoHome;

            if (c.slider != null)
            {
                c.slider.minValue = c.minServo;
                c.slider.maxValue = c.maxServo;

                c.slider.SetValueWithoutNotify(
                    c.servoHome);

                c.slider.onValueChanged.AddListener(
                    value => SetTarget(index, value));
            }

            if (c.joint != null &&
                c.joint.jointType ==
                ArticulationJointType.RevoluteJoint)
            {
                ArticulationDrive drive =
                    c.joint.xDrive;

                drive.stiffness = 10000f;
                drive.damping = 100f;
                drive.forceLimit = 1000f;

                drive.target = ToJointAngle(
                    c, c.servoHome);

                c.joint.xDrive = drive;
            }
        }

        if (homeButton != null)
            homeButton.onClick.AddListener(GoHome);
    }

    private float ToJointAngle(
        JointChannel c,
        float servoAngle)
    {
        return c.jointHome +
            Mathf.Sign(c.direction) *
            (servoAngle - c.servoHome);
    }

    public void SetTarget(int index, float value)
    {
        if (index < 0 || index >= channels.Length)
            return;

        JointChannel c = channels[index];

        if (c == null)
            return;

        c.desired = Mathf.Clamp(
            value, c.minServo, c.maxServo);
    }

    public void GoHome()
    {
        foreach (JointChannel c in channels)
        {
            if (c == null)
                continue;

            c.desired = c.servoHome;

            if (c.slider != null)
            {
                c.slider.SetValueWithoutNotify(
                    c.servoHome);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (JointChannel c in channels)
        {
            if (c == null)
                continue;

            c.commanded = Mathf.MoveTowards(
                c.commanded,
                c.desired,
                Mathf.Max(0f, c.maxSpeed) *
                Time.fixedDeltaTime);

            if (c.joint == null ||
                c.joint.jointType !=
                ArticulationJointType.RevoluteJoint)
                continue;

            ArticulationDrive drive =
                c.joint.xDrive;

            float target = ToJointAngle(
                c, c.commanded);

            if (drive.lowerLimit < drive.upperLimit)
            {
                target = Mathf.Clamp(
                    target,
                    drive.lowerLimit,
                    drive.upperLimit);
            }

            drive.target = target;
            c.joint.xDrive = drive;
        }
    }

    private void Update()
    {
        foreach (JointChannel c in channels)
        {
            if (c == null || c.readout == null)
                continue;

            string measured = "--";

            if (c.joint != null &&
                c.joint.jointType ==
                ArticulationJointType.RevoluteJoint &&
                c.joint.jointPosition.dofCount > 0)
            {
                float angle =
                    c.joint.jointPosition[0] *
                    Mathf.Rad2Deg;

                float servoAngle =
                    c.servoHome +
                    Mathf.Sign(c.direction) *
                    (angle - c.jointHome);

                measured = servoAngle.ToString("F1");
            }

            c.readout.text =
                $"{c.name}: {c.desired:F1}° | " +
                $"Actual: {measured}°";
        }

        if (baseFrame != null &&
            tcpFrame != null &&
            tcpReadout != null)
        {
            Vector3 p =
                baseFrame.InverseTransformPoint(
                    tcpFrame.position);

            Quaternion q =
                Quaternion.Inverse(
                    baseFrame.rotation) *
                tcpFrame.rotation;

            Vector3 r = q.eulerAngles;

            tcpReadout.text =
                $"TCP (m)\n" +
                $"X: {p.x:F3}  " +
                $"Y: {p.y:F3}  " +
                $"Z: {p.z:F3}\n" +
                $"Rotation (°)\n" +
                $"X: {r.x:F1}  " +
                $"Y: {r.y:F1}  " +
                $"Z: {r.z:F1}";
        }
    }
}
