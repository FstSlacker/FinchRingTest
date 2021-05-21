using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RevertDetector
{
    /// <summary>
    /// Angular difference between start ring orientation and tpose orientation along yaw axis in degrees.
    /// </summary>
    private static float RingsToTPoseAngle = 60f;

    /// <summary>
    /// The maximum signed angle value allowed between the directions of the upper arm and hand in degrees.
    /// </summary>
    private static float BorderAngle = 90f;

    /// <summary>
    /// Return is upper arm reverted, call in calibration pose
    /// </summary>
    /// <param name="rawTPosedUpperArm">raw TPosed upper arm rotation from finchcore</param>
    /// <param name="rawTPosedHand">raw TPosed hand rotation from finchcore</param>
    /// <returns> is upper arm reverted</returns>
    public static bool IsUpperArmReverted(Quaternion rawTPosedUpperArm, Quaternion rawTPosedHand)
    {
        Vector3 upperArmOrigin = rawTPosedUpperArm * Vector3.right;
        Vector3 handOrigin = rawTPosedHand * Vector3.right;
        float angularDelta = GetPlaneAngle(upperArmOrigin, handOrigin) + RingsToTPoseAngle;
        return Mathf.Abs(angularDelta) > BorderAngle;
    }

    private static float NormalizeAngle(float angleInDegrees)
    {
        while (angleInDegrees > 180f)
        {
            angleInDegrees -= 360f;
        }
        while (angleInDegrees < -180f)
        {
            angleInDegrees += 360f;
        }
        return angleInDegrees;
    }

    private static float GetPlaneAngle(Vector3 from, Vector3 to)
    {
        Vector3 aProj = Vector3.ProjectOnPlane(from, Vector3.up);
        Vector3 bProj = Vector3.ProjectOnPlane(to, Vector3.up);
        float angle = -Vector3.SignedAngle(aProj, bProj, Vector3.up);
        return NormalizeAngle(angle);
    }
}
