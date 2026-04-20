using UnityEngine;

[CreateAssetMenu(fileName = "Message", menuName = "Scriptable Objects/Message")]
public class Message : ScriptableObject
{   
    public AudioClip audioClip;
    [TextArea] public string message;
    public SignalRequirement[] requirements;

    [Tooltip("Words the player must include in their transcription to be scored correctly.")]
    public string[] keywords;
}
