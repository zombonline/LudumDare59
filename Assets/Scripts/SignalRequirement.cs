using UnityEngine;
[System.Serializable]
public class SignalRequirement
{
    public string label;
    public float targetValue = 0.5f;
    public float bandwidth = 0.1f;

    public float Evaluate(float value)
    {
        return 1f - Mathf.Clamp01(Mathf.Abs(value - targetValue) / bandwidth);
    }
}