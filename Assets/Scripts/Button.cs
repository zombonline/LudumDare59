using System;
using EPOOutline;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable, ISignalSource
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int channelCount = 5;
    [SerializeField] private Sprite[] buttonSprites;
    [SerializeField] private AudioClip[] buttonSounds;
    private int _currentChannel = 0;
    [SerializeField] Outlinable outlinable;
    
    [SerializeField] HandController.HandAnim  handAnim;
    public HandController.HandAnim HandAnim => handAnim;

    [SerializeField] private Transform handPos;
    public Transform? DesiredHandTransform => handPos!=null ? handPos : null;


    public float Value => channelCount <= 1 ? 0f : _currentChannel / (float)(channelCount - 1);

    private void Start()
    {
        spriteRenderer.sprite = buttonSprites[_currentChannel];
    }

    public void OnHoverEnter()
    {
        outlinable.enabled = true;
    }

    public void OnHoverExit()
    {
        outlinable.enabled = false;
    }
    public void OnRelease() {}
    public void OnDrag(Vector2 delta) { }

    public void OnGrab()
    {
        _currentChannel = (_currentChannel + 1) % channelCount;
        spriteRenderer.sprite = buttonSprites[_currentChannel];
        AudioSource.PlayClipAtPoint(buttonSounds[_currentChannel], transform.position);
    }
}