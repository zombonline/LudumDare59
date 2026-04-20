using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    private const string SignalVolumeParam = "SignalVolume";
    private const string InterferenceVolumeParam = "InterferenceVolume";
    private const string SignalDistortionParam = "SignalDistortion";
    
    public static AudioController Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private SignalController signalController;

    [SerializeField] private AudioSource staticSource;
    [SerializeField] private AudioSource messageSource;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }
    private void Update()
    {
        float quality = signalController.SignalQuality;
        mixer.SetFloat(SignalVolumeParam, ToDecibels(quality));
        mixer.SetFloat(InterferenceVolumeParam, ToDecibels(1f - quality));
        mixer.SetFloat(SignalDistortionParam, -quality);
    }

    /// <summary>Converts a normalized 0-1 volume to decibels for AudioMixer parameters.</summary>
    public static float ToDecibels(float normalizedVolume)
    {
        return Mathf.Log10(Mathf.Max(normalizedVolume, 0.0001f)) * 20f;
    }
    
    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        if (newState == GameStateManager.GameState.Tuning)
            staticSource.Play();
        else if (newState == GameStateManager.GameState.Transcribing)
            staticSource.Stop();
    }


    public float GetMessageProgress()
    {
        if(!messageSource.isPlaying)
            return 0f;
        return messageSource.time/messageSource.clip.length;
    }
}