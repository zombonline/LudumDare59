using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class TextCursorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private TextInput textInput;
    [SerializeField] private HandController handController;
    HandController.HandAnim  handAnim = HandController.HandAnim.Pencil ;
    public HandController.HandAnim HandAnim => handAnim;
    public Transform? DesiredHandTransform => holdPoint;
    
    [SerializeField] private Transform textEndPoint;
    [SerializeField] private Transform holdPoint;

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
        textInput.onTextLengthChanged += OnTextLengthChanged;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
        textInput.onTextLengthChanged += OnTextLengthChanged;
    }

    void OnTextLengthChanged(int diff)
    {
        if (diff > 0)
            handAnim = HandController.HandAnim.Pencil;
        if (diff < 0)
            handAnim = HandController.HandAnim.Rubber;
        
        textEndPoint.position = textInput.GetTextEndWorldPos();
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