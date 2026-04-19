using UnityEngine;

public class Button : MonoBehaviour, IInteractable, ISignalSource
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color standardColor, hoveredColor, pressedColor;
    [SerializeField] private int channelCount = 5;

    private int _currentChannel = 0;

    public float Value => channelCount <= 1 ? 0f : _currentChannel / (float)(channelCount - 1);

    public void OnHoverEnter() => spriteRenderer.color = hoveredColor;
    public void OnHoverExit()  => spriteRenderer.color = standardColor;
    public void OnRelease()    => spriteRenderer.color = standardColor;
    public void OnDrag(Vector2 delta) { }
    public Vector3? DesiredHandPosition { get; }

    public void OnGrab()
    {
        _currentChannel = (_currentChannel + 1) % channelCount;
        spriteRenderer.color = pressedColor;
    }
}