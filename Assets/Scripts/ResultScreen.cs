using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private GeminiCore geminiCore;
    [FormerlySerializedAs("continuePrompt")] [SerializeField] private GameObject feedbackDisplay;

    private bool _readyToContinue;

    private void Start()
    {
        geminiCore.OnResponseReady += OnResponseReady;
        GameStateManager.Instance.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        geminiCore.OnResponseReady -= OnResponseReady;
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameStateManager.GameState state)
    {
        if (state != GameStateManager.GameState.Result)
        {
            _readyToContinue = false;
            feedbackDisplay?.SetActive(false);
        }
    }

    private void OnResponseReady()
    {
        _readyToContinue = true;
        feedbackDisplay?.SetActive(true);
    }

    private void Update()
    {
        if (!_readyToContinue) return;
        if (GameStateManager.Instance.CurrentState != GameStateManager.GameState.Result) return;

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            _readyToContinue = false;
            GameStateManager.Instance.NextMessage();
        }
    }
}
