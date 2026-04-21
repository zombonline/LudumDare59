using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState { Tuning, Transcribing, Result }

    public static GameStateManager Instance { get; private set; }
    public event Action<GameState> OnStateChanged;
    public GameState CurrentState { get; private set; }
    [SerializeField] private Message[] messages;
    private int _currentMessageIndex = 0;
    public Message CurrentMessage => messages[_currentMessageIndex];

    public float TimeRemaining { get; private set; }

    [SerializeField] private Transform tuningScreen, transcribingScreen, resultScreen;

    [Header("Signal")]
    [SerializeField] private SignalController signalController;
    [SerializeField] private AudioSource messageAudioSource;
    [Range(0f, 1f)]
    [SerializeField] private float messageThreshold = 0.7f;
    [SerializeField] private float messageStartDelay = 1.5f;

    [Header("Transcribing")]
    [SerializeField] private float transcribeTimeLimit = 60f;
    [SerializeField] private TextInput transcribeInput;

    private bool _messageStarted = false;
    private float _tuningLockoutTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SetState(GameState.Tuning);
        transcribeInput.onEnterPressed += OnTranscribeComplete;
    }

    private void OnDestroy()
    {
        transcribeInput.onEnterPressed -= OnTranscribeComplete;
    }

    private void OnTranscribeComplete()
    {
        SetState(GameState.Result);
    }

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
        if (_tuningLockoutTimer > 0f)
        {
            _tuningLockoutTimer -= Time.deltaTime;
            return;
        }

        if (!_messageStarted && signalController.SignalQuality >= messageThreshold)
        {
            _messageStarted = true;
            messageAudioSource.clip = messages[_currentMessageIndex].audioClip;
            messageAudioSource.Play();
        }

        if (_messageStarted && !messageAudioSource.isPlaying && Application.isFocused)
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
    
    /// <summary>Called by the Result screen continue button to load the next message.</summary>
    public void NextMessage()
    {
        _currentMessageIndex++;

        if (_currentMessageIndex >= messages.Length)
        {
            // TODO: end game / credits
            Debug.Log("[GameStateManager] All messages complete.");
            return;
        }

        _messageStarted = false;
        transcribeInput.Clear();
        SetState(GameState.Tuning);
    }


    public void Submit() => SetState(GameState.Result);

    /// <summary>Ends the game and returns to the intro scene.</summary>
    public void GameOver() => SceneLoader.LoadIntro();

    private void SetState(GameState newState)
    {
        if (newState == GameState.Tuning)
        {
            signalController.LoadRequirements(messages[_currentMessageIndex]);
            _tuningLockoutTimer = messageStartDelay;
        }
        CurrentState = newState;
        if (newState == GameState.Transcribing)
            TimeRemaining = transcribeTimeLimit;

        OnStateChanged?.Invoke(newState);

        // tuningScreen.transform.position = newState == GameState.Tuning ? Vector3.zero : Vector3.up * 100;
        transcribingScreen.transform.position = newState == GameState.Transcribing ? Vector3.zero : Vector3.up * 100;
        resultScreen.transform.position = newState == GameState.Result ? Vector3.zero : Vector3.up * 100;
    }
}
