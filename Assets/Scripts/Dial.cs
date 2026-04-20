using UnityEngine;

public class Dial : MonoBehaviour, IInteractable, ISignalSource
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minAngle = -135f;
    [SerializeField] private float maxAngle = 135f;
    [SerializeField] private bool useYRotation, useXRotation;
    private float currentAngle = 0f;
    [SerializeField] private Transform handPos;

    public void OnHoverEnter()
    {
    }

    public void OnHoverExit()
    {
    }

    public void OnGrab()
    {
    }

    public void OnRelease()
    {
    }

    public void OnDrag(Vector2 worldDelta)
    {
        currentAngle = useXRotation? Mathf.Clamp(currentAngle - worldDelta.x * sensitivity, minAngle, maxAngle) : currentAngle;
        currentAngle = useYRotation? Mathf.Clamp(currentAngle + worldDelta.y * sensitivity, minAngle, maxAngle) : currentAngle;
        spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public Vector3? DesiredHandPosition => handPos!=null ? handPos.position : null;

    public float Value => Mathf.InverseLerp(minAngle, maxAngle, currentAngle);


}
