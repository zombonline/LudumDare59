using UnityEngine;

public interface IInteractable
{
    public void OnHoverEnter();
    public void OnHoverExit();
    public void OnGrab();
    public void OnRelease();
    public void OnDrag(Vector2 delta);
    Transform? DesiredHandTransform { get; }
    HandController.HandAnim HandAnim { get; }
}
