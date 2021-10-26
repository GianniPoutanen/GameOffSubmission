using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    IEnumerator currentShake;

    public void StartShake(ShakeProperties properties)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);
        currentShake = Shake(properties);
        StartCoroutine(currentShake);
    }

    IEnumerator Shake(ShakeProperties properties)
    {
        float completionPercent = 0;
        float movePercent = 0;

        float angle_radians = properties.angle * Mathf.Deg2Rad - Mathf.PI;
        Vector3 prevWaypoint = Vector3.zero;
        Vector3 curWaypoint = Vector3.zero;

        float moveDistance = 0;

        Quaternion targetRotation = Quaternion.identity;
        Quaternion prevRotation = Quaternion.identity;

        do
        {
            if (movePercent >= 1 || completionPercent == 0)
            {
                float dampingFactor = DampingCurve(completionPercent, properties.dampingPercent);
                float noiseAngle = (Random.value - 0.5f) * Mathf.PI;
                angle_radians += Mathf.PI + noiseAngle * properties.noisePercent;

                curWaypoint = new Vector3(Mathf.Cos(angle_radians), Mathf.Sin(angle_radians)) * properties.strength * dampingFactor;
                prevWaypoint = this.transform.localPosition;

                targetRotation = Quaternion.Euler(new Vector3(curWaypoint.y, curWaypoint.x).normalized * properties.rotationPercent * properties.dampingPercent);
                prevRotation = this.transform.localRotation;

                moveDistance = Vector3.Distance(curWaypoint, prevWaypoint);
                movePercent = 0;
            }


            completionPercent += Time.deltaTime / properties.duration;
            movePercent += Time.deltaTime / moveDistance * properties.speed;
            transform.localPosition = Vector3.Lerp(prevWaypoint, curWaypoint, movePercent);
            transform.localRotation = Quaternion.Slerp(prevRotation, targetRotation, movePercent);

            yield return null;
        }
        while (moveDistance > 0);
    }

    private float DampingCurve(float x, float dampingPercent)
    {
        x = Mathf.Clamp01(x);
        float a = Mathf.Lerp(2, 0.25f, dampingPercent);
        float b = 1 - Mathf.Pow(x, a);
        return b * b * b;
    }

    [System.Serializable]
    public class ShakeProperties
    {
        public float angle;
        public float strength;
        public float speed;
        public float duration;
        [Range(0, 1)]
        public float noisePercent;
        [Range(0, 1)]
        public float dampingPercent;
        [Header("For 3D Cameras")]
        public float maxAngle;
        [Range(0, 1)]
        public float rotationPercent; // for 3d camera

        public ShakeProperties(float angle, float strength, float speed, float duration, float noisePercent, float dampingPercent, float maxAngle, float rotationPercent)
        {
            this.angle = angle;
            this.strength = strength;
            this.speed = speed;
            this.duration = duration;
            this.noisePercent = noisePercent;
            this.dampingPercent = dampingPercent;
            this.maxAngle = maxAngle;
            this.rotationPercent = rotationPercent;
        }
    }
}
