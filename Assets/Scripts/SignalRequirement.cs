using UnityEngine;
[System.Serializable]
public class SignalRequirement
{
    public string label;
    public AnimationCurve targetValueCurve;
    public float bandwidth = 0.1f;
    
    public float Evaluate(float value, float messageProgress)
    {
        return 1f - Mathf.Clamp01(Mathf.Abs(value - targetValueCurve.Evaluate(messageProgress)) / bandwidth);
    }
}