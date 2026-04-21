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

    [Header("Difficulty")]
    [Tooltip("Maximum normalised volume the signal can reach even at perfect tuning (0–1). Lower = harder.")]
    [SerializeField] [Range(0.01f, 1f)] private float signalVolumeMax = 0.45f;
    [Tooltip("Minimum normalised volume the interference holds even at perfect tuning (0–1). Higher = harder.")]
    [SerializeField] [Range(0f, 1f)] private float interferenceVolumeMin = 0.6f;

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

        float signalNorm      = quality * signalVolumeMax;
        float interferenceNorm = Mathf.Lerp(1f, interferenceVolumeMin, quality);

        mixer.SetFloat(SignalVolumeParam,       ToDecibels(signalNorm));
        mixer.SetFloat(InterferenceVolumeParam, ToDecibels(interferenceNorm));
        mixer.SetFloat(SignalDistortionParam,   -quality);
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