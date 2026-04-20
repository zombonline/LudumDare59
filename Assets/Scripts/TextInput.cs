using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TextInput : MonoBehaviour
{
    [SerializeField] private TextMeshPro textComponent;
    public event Action onEnterPressed;

    public string Text { get; private set; } = "";

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;

        Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.Transcribing)
            Keyboard.current.onTextInput += OnTextInput;
        else
            Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnTextInput(char c)
    {
        if (char.IsControl(c)) return;
        SetText(Text + c);
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameStateManager.GameState.Transcribing)
            return;

        if (Keyboard.current.backspaceKey.wasPressedThisFrame && Text.Length > 0)
            SetText(Text[..^1]);
        if (Keyboard.current.enterKey.wasPressedThisFrame && Text.Length > 0)
        {
            Debug.Log("Enter pressed");
            onEnterPressed?.Invoke();
        }
    }

    private void SetText(string newText)
    {
        Text = newText;
        textComponent.text = Text;
        textComponent.ForceMeshUpdate();
    }
    
    public void Clear() => SetText("");


    /// <summary>Returns the world position of the end of the current text.</summary>
    public Vector3 GetTextEndWorldPos()
    {
        TMP_TextInfo info = textComponent.textInfo;

        if (info.characterCount == 0)
            return textComponent.transform.position;

        TMP_CharacterInfo lastChar = info.characterInfo[info.characterCount - 1];
        Vector3 localPos = (lastChar.topRight + lastChar.bottomRight) / 2f;
        return textComponent.transform.TransformPoint(localPos);
    }
    
}