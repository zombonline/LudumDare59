using UnityEngine;
[System.Serializable]
public class SignalRequirement
{
    public string label;
    public AnimationCurve targetValueCurve;
    public float bandwidth = 0.1f;
    
    public float Evaluate(ISignalSource source, float messageProgress)
    {
        float target = source.SnapToAchievable(targetValueCurve.Evaluate(messageProgress));
        return 1f - Mathf.Clamp01(Mathf.Abs(source.Value - target) / bandwidth);
    }
}