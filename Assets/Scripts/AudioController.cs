using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    private const string SignalVolumeParam = "SignalVolume";
    private const string InterferenceVolumeParam = "InterferenceVolume";
    private const string SignalDistortionParam = "SignalDistortion";

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private SignalController signalController;

    [SerializeField] private AudioSource staticSource;
    
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += OnGameStateChaned;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= OnGameStateChaned;
    }
    private void Update()
    {
        float quality = signalController.SignalQuality;
        mixer.SetFloat(SignalVolumeParam, ToDecibels(quality));
        mixer.SetFloat(InterferenceVolumeParam, ToDecibels(1f - quality));
        mixer.SetFloat(SignalDistortionParam, -quality);
    }

    /// <summary>Converts a normalized 0-1 volume to decibels for AudioMixer parameters.</summary>
    private float ToDecibels(float normalizedVolume)
    {
        return Mathf.Log10(Mathf.Max(normalizedVolume, 0.0001f)) * 20f;
    }
    
    private void OnGameStateChaned(GameStateManager.GameState newState)
    {
        staticSource.mute = newState == GameStateManager.GameState.Tuning;
    }
}