using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MessageInput : MonoBehaviour
{
    [SerializeField] EventSystem  eventSystem;
    TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        if(newState != GameStateManager.GameState.Transcribing)
            return;
        eventSystem.SetSelectedGameObject(inputField.gameObject, null);
        inputField.OnPointerClick(null);
    }
}
