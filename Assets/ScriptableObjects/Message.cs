using UnityEngine;

[CreateAssetMenu(fileName = "Message", menuName = "Scriptable Objects/Message")]
public class Message : ScriptableObject
{   
    public AudioClip audioClip;
    [TextArea]public string message;
    public SignalRequirement[] requirements;
}
