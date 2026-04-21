using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    private enum HandState { Free, Hovering, Holding }
    public enum HandAnim {Click, Grab, Release, Pinch, Pencil, Rubber}
    [SerializeField] private Sprite click, grab, release, pinch, pencil, rubber;

    private HandState State { get; set; }

    private IInteractable currentHovered;
    private IInteractable heldInteractable;
    private Camera mainCamera;
    [SerializeField] private float sensitivity;
    [SerializeField] private SpriteRenderer spriteObject;

    
    [SerializeField] private PolygonCollider2D handCollider;
    [SerializeField] private float colliderScale = 0.8f;


    private static readonly ContactFilter2D InteractableFilter = new ContactFilter2D().NoFilter();
    private readonly List<Collider2D> _overlapResults = new List<Collider2D>();

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        transform.position = Vector3.zero;
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
        UpdateColliderShape(spriteObject.sprite);

    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        Vector2 pixelDelta = Mouse.current.delta.ReadValue();
        Vector3 worldDelta = PixelDeltaToWorld(pixelDelta);
        
        UpdateSprite();

        if (State == HandState.Holding)
        {
            heldInteractable.OnDrag(worldDelta);

            if (heldInteractable.DesiredHandTransform)
            {
                transform.position = heldInteractable.DesiredHandTransform.position;
                transform.localRotation = heldInteractable.DesiredHandTransform.rotation;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame
                && GameStateManager.Instance.CurrentState == GameStateManager.GameState.Tuning)
                Release();

            return;
        }
        
        
        transform.position += worldDelta * sensitivity;

        IInteractable interactable = null;
        Physics2D.OverlapCollider(handCollider, InteractableFilter, _overlapResults);
        foreach (var col in _overlapResults)
        {
            interactable = col.GetComponentInParent<IInteractable>();
            if (interactable != null) break;
        }



        if (interactable != currentHovered)
        {
            currentHovered?.OnHoverExit();
            currentHovered = interactable;
            currentHovered?.OnHoverEnter();
            State = currentHovered != null ? HandState.Hovering : HandState.Free;
        }

        if (currentHovered != null && Mouse.current.leftButton.wasPressedThisFrame)
            Grab(currentHovered);
        ClampToScreen();
    }

    public void Grab(IInteractable interactable)
    {
        if (heldInteractable != null)
        {
            heldInteractable.OnRelease();
            heldInteractable.OnHoverExit();
        }
        heldInteractable = interactable;
        State = HandState.Holding;
        heldInteractable.OnGrab();

    }

    public void Release()
    {
        heldInteractable?.OnRelease();
        heldInteractable = null;
        transform.rotation = Quaternion.identity;
        State = currentHovered != null ? HandState.Hovering : HandState.Free;

    }

    private void UpdateSprite()
    {
        var anim = heldInteractable?.HandAnim ?? HandAnim.Release;
        switch (anim)
        {
            case HandAnim.Click:
                spriteObject.sprite = click;
                break;
            case HandAnim.Grab:
                spriteObject.sprite = grab;
                break;
            case HandAnim.Release:
                spriteObject.sprite = release;
                break;
            case HandAnim.Pinch:
                spriteObject.sprite = pinch;
                break;
            case HandAnim.Pencil:
                spriteObject.sprite = pencil;
                break;
            case HandAnim.Rubber:
                spriteObject.sprite = rubber;
                break;
        }
        UpdateColliderShape(spriteObject.sprite);
    }

    private Vector3 PixelDeltaToWorld(Vector2 pixelDelta)
    {
        float pixelsPerWorldUnit = Screen.height / (mainCamera.orthographicSize * 2f);
        return new Vector3(pixelDelta.x, pixelDelta.y, 0f) / pixelsPerWorldUnit;
    }
    
    private void ClampToScreen()
    {
        Vector3 min = mainCamera.ViewportToWorldPoint(Vector3.zero);
        Vector3 max = mainCamera.ViewportToWorldPoint(Vector3.one);
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, min.x, max.x),
            Mathf.Clamp(transform.position.y, min.y, max.y),
            0f
        );
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state != GameStateManager.GameState.Tuning)
        {
            Cursor.lockState = CursorLockMode.None;
            currentHovered?.OnHoverExit();
            currentHovered = null;
            heldInteractable?.OnRelease();
            heldInteractable = null;
            State = HandState.Free;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        spriteObject.gameObject.SetActive(state != GameStateManager.GameState.Result);
    }
    
    private void UpdateColliderShape(Sprite sprite)
    {
        var path = new List<Vector2>();
        handCollider.pathCount = sprite.GetPhysicsShapeCount();
        for (int i = 0; i < sprite.GetPhysicsShapeCount(); i++)
        {
            sprite.GetPhysicsShape(i, path);
            for (int j = 0; j < path.Count; j++)
                path[j] *= colliderScale;
            handCollider.SetPath(i, path);
        }
    }

}
