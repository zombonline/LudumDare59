using System;
using UnityEngine;

public class Slider : MonoBehaviour, IInteractable, ISignalSource
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Handle")]
    [SerializeField] private Transform handle;
    [SerializeField] private float slideRange = 1f;
    [SerializeField] private bool vertical = true;

    [Header("Sensitivity")]
    [SerializeField] private float sensitivity = 1f;

    [Header("Increments")]
    [SerializeField] private bool useIncrements = false;
    [SerializeField] private int incrementCount = 5;

    private float _rawValue = 0.5f;

    public float Value
    {
        get
        {
            if (!useIncrements || incrementCount <= 1)
                return _rawValue;

            float step = 1f / (incrementCount - 1);
            return Mathf.Round(_rawValue / step) * step;
        }
    }

    private void Start()
    {
        UpdateHandlePosition();
    }

    public void OnHoverEnter()
    {
    }

    public void OnHoverExit() {}
    public void OnGrab() {}

    public void OnRelease()
    {
        UpdateHandlePosition(false);
    }

    public void OnDrag(Vector2 worldDelta)
    {
        float delta = vertical ? worldDelta.y : worldDelta.x;
        _rawValue = Mathf.Clamp01(_rawValue + delta * sensitivity);
        UpdateHandlePosition(true);
    }

    public Vector3? DesiredHandPosition => handle.position;

    private void UpdateHandlePosition(bool useRawValue = false)
    {
        if (handle == null) return;

        float offset = ((useRawValue ? _rawValue : Value ) - 0.5f) * slideRange;
        handle.localPosition = vertical
            ? new Vector3(0f, offset, 0f)
            : new Vector3(offset, 0f, 0f);
    }

    private void OnDrawGizmos()
    {
        //draw range
        Vector2 left = new Vector2(transform.position.x+(-slideRange / 2f), transform.position.y);
        Vector2 right = new Vector2(transform.position.x+(slideRange / 2f), transform.position.y);
        
        Gizmos.DrawLine(left,right);
    }
}