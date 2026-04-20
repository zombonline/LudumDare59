using UnityEngine;

/// <summary>Moves a needle transform along a local axis to display a 0–1 signal value on a linear meter.</summary>
public class SliderMeterDisplay : MonoBehaviour, ISignalDisplay
{
    public enum Axis { X, Y }

    [SerializeField] private Transform needle;
    [SerializeField] private Axis axis = Axis.X;
    [SerializeField] private float minPosition = -1f;
    [SerializeField] private float maxPosition =  1f;
    [SerializeField] private float smoothSpeed  = 8f;

    private float _targetPosition;

    private void Awake()
    {
        _targetPosition = minPosition;
    }

    /// <summary>Updates the needle position to reflect the given 0–1 signal quality value.</summary>
    public void UpdateDisplay(float value)
    {
        _targetPosition = Mathf.Lerp(minPosition, maxPosition, Mathf.Clamp01(value));
    }

    private void Update()
    {
        Vector3 local = needle.localPosition;
        if (axis == Axis.X)
            local.x = Mathf.Lerp(local.x, _targetPosition, Time.deltaTime * smoothSpeed);
        else
            local.y = Mathf.Lerp(local.y, _targetPosition, Time.deltaTime * smoothSpeed);
        needle.localPosition = local;
    }
}
