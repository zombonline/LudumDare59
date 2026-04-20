using System.Collections;
using UnityEngine;

public class TextCursorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private TextInput textInput;
    [SerializeField] private HandController handController;

    public Vector3? DesiredHandPosition => textInput.GetTextEndWorldPos();

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.Transcribing)
            StartCoroutine(GrabNextFrame());
        else
            handController.Release();
    }

    private IEnumerator GrabNextFrame()
    {
        yield return null;
        handController.Grab(this);
    }

    public void OnHoverEnter() { }
    public void OnHoverExit()  { }
    public void OnGrab()       { }
    public void OnRelease()    { }
    public void OnDrag(Vector2 delta) { }
}