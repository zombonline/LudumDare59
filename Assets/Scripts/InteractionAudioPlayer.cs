using System.Collections;
using UnityEngine;

/// <summary>Plays a looping audio clip with ADSR volume shaping driven by interactable drag movement.</summary>
public class InteractionAudioPlayer : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("ADSR")]
    [SerializeField] private float attackTime  = 0.05f;
    [SerializeField] private float decayTime   = 0.1f;
    [SerializeField] private float sustainBase = 0.4f;   // volume floor while held
    [SerializeField] private float sustainMax  = 1.0f;   // volume when moving fast
    [SerializeField] private float releaseTime = 0.2f;

    [Header("Movement Response")]
    [Tooltip("Movement magnitude that maps to maximum sustain volume.")]
    [SerializeField] private float movementScale = 0.05f;

    private float _currentVolume;
    private float _movementMagnitude;
    private bool  _isHeld;
    private Coroutine _adsrRoutine;

    /// <summary>Call from IInteractable.OnGrab.</summary>
    public void OnGrab()
    {
        _isHeld = true;
        _movementMagnitude = 0f;
        audioSource.volume = 0f;
        audioSource.Play();
        RestartRoutine(AttackDecayRoutine());
    }

    /// <summary>Call from IInteractable.OnRelease.</summary>
    public void OnRelease()
    {
        _isHeld = false;
        RestartRoutine(ReleaseRoutine());
    }

    /// <summary>Call from IInteractable.OnDrag with the world-space delta.</summary>
    public void OnDrag(Vector2 worldDelta)
    {
        _movementMagnitude = worldDelta.magnitude;
    }

    private void Update()
    {
        if (!_isHeld) return;

        // Sustain volume tracks movement magnitude smoothly
        float targetSustain = Mathf.Lerp(sustainBase, sustainMax,
            Mathf.Clamp01(_movementMagnitude / movementScale));
        _currentVolume = Mathf.Lerp(_currentVolume, targetSustain, Time.deltaTime * 10f);
    }

    private IEnumerator AttackDecayRoutine()
    {
        // Attack
        float t = 0f;
        float startVolume = audioSource.volume;
        while (t < attackTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, sustainMax, t / attackTime);
            yield return null;
        }

        // Decay to base sustain
        t = 0f;
        while (t < decayTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(sustainMax, sustainBase, t / decayTime);
            yield return null;
        }

        // Hand off volume control to Update (sustain phase)
        _currentVolume = sustainBase;
        while (_isHeld)
        {
            audioSource.volume = _currentVolume;
            yield return null;
        }
    }

    private IEnumerator ReleaseRoutine()
    {
        float startVolume = audioSource.volume;
        float t = 0f;
        while (t < releaseTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / releaseTime);
            yield return null;
        }
        audioSource.Stop();
    }

    private void RestartRoutine(IEnumerator routine)
    {
        if (_adsrRoutine != null) StopCoroutine(_adsrRoutine);
        _adsrRoutine = StartCoroutine(routine);
    }
}
