using System;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable, ISignalSource
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int channelCount = 5;
    [SerializeField] private Sprite[] buttonSprites;
    [SerializeField] private AudioClip[] buttonSounds;
    private int _currentChannel = 0;

    public float Value => channelCount <= 1 ? 0f : _currentChannel / (float)(channelCount - 1);

    private void Start()
    {
        spriteRenderer.sprite = buttonSprites[_currentChannel];
    }

    public void OnHoverEnter()
    {
    }

    public void OnHoverExit(){}
    public void OnRelease() {}
    public void OnDrag(Vector2 delta) { }
    public Vector3? DesiredHandPosition { get; }

    public void OnGrab()
    {
        _currentChannel = (_currentChannel + 1) % channelCount;
        spriteRenderer.sprite = buttonSprites[_currentChannel];
        AudioSource.PlayClipAtPoint(buttonSounds[_currentChannel], transform.position);
    }
}