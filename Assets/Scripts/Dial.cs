using EPOOutline;
using UnityEngine;

public class Dial : MonoBehaviour, IInteractable, ISignalSource
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minAngle = -135f;
    [SerializeField] private float maxAngle = 135f;
    [SerializeField] private bool useYRotation, useXRotation;
    private float currentAngle = 0f;


    [SerializeField] private int lockCount = -1;
    [SerializeField] Outlinable  outlinable;
    [SerializeField] HandController.HandAnim  handAnim;
    public HandController.HandAnim HandAnim => handAnim;
    
    [SerializeField] private Transform handPos;
    public Transform? DesiredHandTransform => handPos!=null ? handPos : null;
    
    [SerializeField] private InteractionAudioPlayer audioPlayer;
    public void OnHoverEnter()
    {
        outlinable.enabled = true;
    }

    public void OnHoverExit()
    {
        outlinable.enabled = false;
    }

    public void OnGrab()
    {
        audioPlayer?.OnGrab();
    }

    public void OnRelease()
    {
        if (lockCount > 0)
            SnapToLockPoint();
        audioPlayer?.OnRelease();
    }

    public void OnDrag(Vector2 worldDelta)
    {
        currentAngle = useXRotation? Mathf.Clamp(currentAngle - worldDelta.x * sensitivity, minAngle, maxAngle) : currentAngle;
        currentAngle = useYRotation? Mathf.Clamp(currentAngle + worldDelta.y * sensitivity, minAngle, maxAngle) : currentAngle;
        spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
        
        audioPlayer?.OnDrag(worldDelta);
    }

    private void SnapToLockPoint()
    {
        float nearestAngle = minAngle;
        float smallestDelta = float.MaxValue;
        for (int i = 0; i < lockCount; i++)
        {
            float lockAngle = Mathf.Lerp(minAngle, maxAngle, (float)i / (lockCount - 1));
            float delta = Mathf.Abs(currentAngle - lockAngle);
            if (delta < smallestDelta)
            {
                smallestDelta = delta;
                nearestAngle = lockAngle;
            }
        }
        currentAngle = nearestAngle;
        spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
    public float SnapToAchievable(float value)
    {
        if (lockCount <= 1) return value;

        float nearest = 0f;
        float smallestDelta = float.MaxValue;

        for (int i = 0; i < lockCount; i++)
        {
            float lockValue = (float)i / (lockCount - 1);
            float delta = Mathf.Abs(value - lockValue);
            if (delta < smallestDelta)
            {
                smallestDelta = delta;
                nearest = lockValue;
            }
        }

        return nearest;
    }



    public float Value => Mathf.InverseLerp(minAngle, maxAngle, currentAngle);


}
