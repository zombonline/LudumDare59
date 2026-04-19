using UnityEngine;

public class Dial : MonoBehaviour, IInteractable, ISignalSource
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Color standardColor, hoveredColor, pressedColor;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minAngle = -135f;
    [SerializeField] private float maxAngle = 135f;
    private float currentAngle = 0f;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnHoverEnter()
    {
        spriteRenderer.color = hoveredColor;
    }

    public void OnHoverExit()
    {
        spriteRenderer.color = standardColor;
    }

    public void OnGrab()
    {
        spriteRenderer.color = pressedColor;
    }

    public void OnRelease()
    {
        spriteRenderer.color = standardColor;
    }

    public void OnDrag(Vector2 worldDelta)
    {
        currentAngle = Mathf.Clamp(currentAngle - worldDelta.x * sensitivity, minAngle, maxAngle);
        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public Vector3? DesiredHandPosition { get; }

    public float Value => Mathf.InverseLerp(minAngle, maxAngle, currentAngle);


}
