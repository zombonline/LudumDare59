using UnityEngine;

[CreateAssetMenu(fileName = "InterferenceSet", menuName = "Signal/Interference Set")]
public class InterferenceSet : ScriptableObject
{
    public AudioClip[] clips;
    public Vector2 playIntervalRange = new Vector2(15f, 35f);
}
