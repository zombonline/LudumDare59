using UnityEngine;

/// <summary>Rotates a needle transform to display a 0–1 signal value on a circular/dial meter.</summary>
public class CircularMeterDisplay : MonoBehaviour, ISignalDisplay
{
    [SerializeField] private Transform needle;
    [SerializeField] private float minAngle = 135f;
    [SerializeField] private float maxAngle = -135f;
    [SerializeField] private float smoothSpeed = 8f;

    private float _targetAngle;

    private void Awake()
    {
        _targetAngle = minAngle;
    }

    /// <summary>Updates the needle rotation to reflect the given 0–1 signal quality value.</summary>
    public void UpdateDisplay(float value)
    {
        _targetAngle = Mathf.Lerp(minAngle, maxAngle, Mathf.Clamp01(value));
    }

    private void Update()
    {
        float current = needle.localEulerAngles.z;
        float smoothed = Mathf.LerpAngle(current, _targetAngle, Time.deltaTime * smoothSpeed);
        needle.localEulerAngles = new Vector3(0f, 0f, smoothed);
    }
}
