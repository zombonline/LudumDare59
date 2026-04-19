using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState { Tuning, Transcribing, Result }

    public static GameStateManager Instance { get; private set; }

    public event Action<GameState> OnStateChanged;
    public GameState CurrentState { get; private set; }
    public float TimeRemaining { get; private set; }

    [SerializeField] private Transform tuningScreen, transcribingScreen;

    [Header("Signal")]
    [SerializeField] private SignalController signalController;
    [SerializeField] private AudioSource messageAudioSource;
    [Range(0f, 1f)]
    [SerializeField] private float messageThreshold = 0.7f;

    [Header("Transcribing")]
    [SerializeField] private float transcribeTimeLimit = 60f;

    private bool _messageStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start() => SetState(GameState.Tuning);

    private void Update()
    {
        switch (CurrentState)
        {
            case GameState.Tuning:      UpdateTuning();      break;
            case GameState.Transcribing: UpdateTranscribing(); break;
        }
    }

    private void UpdateTuning()
    {
        if (!_messageStarted && signalController.SignalQuality >= messageThreshold)
        {
            _messageStarted = true;
            messageAudioSource.Play();
        }

        if (_messageStarted && !messageAudioSource.isPlaying)
            SetState(GameState.Transcribing);
    }

    private void UpdateTranscribing()
    {
        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            Submit();
        }
    }

    public void Submit() => SetState(GameState.Result);

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        if (newState == GameState.Transcribing)
            TimeRemaining = transcribeTimeLimit;

        OnStateChanged?.Invoke(newState);

        tuningScreen.gameObject.SetActive(newState == GameState.Tuning);
        transcribingScreen.gameObject.SetActive(newState == GameState.Transcribing);
    }
}
