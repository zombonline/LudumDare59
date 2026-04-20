using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    private enum HandState { Free, Hovering, Holding }

    private HandState State { get; set; }

    private IInteractable currentHovered;
    private IInteractable heldInteractable;
    private Camera mainCamera;
    [SerializeField] private float sensitivity;
    [SerializeField] private GameObject spriteObject;

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        transform.position = Vector3.zero;
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        Vector2 pixelDelta = Mouse.current.delta.ReadValue();
        Vector3 worldDelta = PixelDeltaToWorld(pixelDelta);

        if (State == HandState.Holding)
        {
            heldInteractable.OnDrag(worldDelta);
            
            if(heldInteractable.DesiredHandPosition.HasValue)
                transform.position = heldInteractable.DesiredHandPosition.Value;
            
            if (Mouse.current.leftButton.wasReleasedThisFrame
                && GameStateManager.Instance.CurrentState == GameStateManager.GameState.Tuning)
                Release();

            return;
        }

        transform.position += worldDelta * sensitivity;

        Collider2D hit = Physics2D.OverlapPoint(transform.position);
        IInteractable interactable = hit?.GetComponentInParent<IInteractable>();



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
        heldInteractable = interactable;
        State = HandState.Holding;
        heldInteractable.OnGrab();
    }

    public void Release()
    {
        heldInteractable?.OnRelease();
        heldInteractable = null;
        State = currentHovered != null ? HandState.Hovering : HandState.Free;
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
        
        spriteObject.SetActive(state != GameStateManager.GameState.Result);
    }

}
