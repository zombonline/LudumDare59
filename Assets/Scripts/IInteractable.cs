using UnityEngine;

public interface IInteractable
{
    public void OnHoverEnter();
    public void OnHoverExit();
    public void OnGrab();
    public void OnRelease();
    public void OnDrag(Vector2 delta);
    Vector3? DesiredHandPosition { get; }
}
