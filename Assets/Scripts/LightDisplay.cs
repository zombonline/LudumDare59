using UnityEngine;

public class LightDisplay : MonoBehaviour, ISignalDisplay
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite on, off;

    public void UpdateDisplay(float value)
    {
        if(Mathf.Approximately(value, 1f))
            spriteRenderer.sprite = on;
        else 
            spriteRenderer.sprite = off;
    }
}
