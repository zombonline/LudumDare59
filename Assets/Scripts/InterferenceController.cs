using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class InterferenceController : MonoBehaviour
{
    private const string ChatterVolumeParam = "ChatterVolume";

    [SerializeField] private InterferenceSet[] sets;
    [SerializeField] private AudioSource chatterSource;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private SignalController signalController;

    private SignalRequirement[] _conflictingRequirements;
    private Coroutine _playCoroutine;

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
        SetChatterVolume(0f);
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameStateManager.GameState.Tuning) return;
        if (_conflictingRequirements == null || _conflictingRequirements.Length == 0) return;

        SetChatterVolume(CalculateInterferenceQuality());
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.Tuning)
        {
            _conflictingRequirements = GenerateConflictingRequirements(
                GameStateManager.Instance.CurrentMessage.requirements);

            if (_playCoroutine != null) StopCoroutine(_playCoroutine);
            _playCoroutine = StartCoroutine(PlayRoutine());
        }
        else
        {
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
                _playCoroutine = null;
            }
            chatterSource.Stop();
            SetChatterVolume(0f);
        }
    }

    private IEnumerator PlayRoutine()
    {
        if (sets == null || sets.Length == 0) yield break;

        while (true)
        {
            InterferenceSet set = sets[Random.Range(0, sets.Length)];

            if (set.clips == null || set.clips.Length == 0)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            float delay = Random.Range(set.playIntervalRange.x, set.playIntervalRange.y);
            yield return new WaitForSeconds(delay);

            chatterSource.clip = set.clips[Random.Range(0, set.clips.Length)];
            chatterSource.Play();

            yield return new WaitWhile(() => chatterSource.isPlaying && Application.isFocused);
        }
    }

    private float CalculateInterferenceQuality()
    {
        float total = 0f;
        int count = 0;
        float progress = AudioController.Instance.GetMessageProgress();

        foreach (var req in _conflictingRequirements)
        {
            foreach (var binding in signalController.Bindings)
            {
                if (binding.label != req.label || binding.SignalSource == null) continue;

                total += req.Evaluate(binding.SignalSource.Value, progress);
                count++;
                break;
            }
        }

        return count > 0 ? total / count : 0f;
    }

    private void SetChatterVolume(float normalizedVolume)
    {
        mixer.SetFloat(ChatterVolumeParam, AudioController.ToDecibels(normalizedVolume));
    }

    private SignalRequirement[] GenerateConflictingRequirements(SignalRequirement[] messageRequirements)
    {
        if (messageRequirements == null || messageRequirements.Length == 0)
            return System.Array.Empty<SignalRequirement>();

        float shift = Random.Range(0.4f, 0.6f);
        var result = new SignalRequirement[messageRequirements.Length];

        for (int i = 0; i < messageRequirements.Length; i++)
        {
            var original = messageRequirements[i];
            result[i] = new SignalRequirement
            {
                label = original.label,
                bandwidth = original.bandwidth,
                targetValueCurve = ShiftCurve(original.targetValueCurve, shift)
            };
        }

        return result;
    }

    private AnimationCurve ShiftCurve(AnimationCurve original, float shift)
    {
        const int SampleCount = 8;
        var curve = new AnimationCurve();

        for (int i = 0; i <= SampleCount; i++)
        {
            float t = i / (float)SampleCount;
            float shifted = (original.Evaluate(t) + shift) % 1f;
            curve.AddKey(t, shifted);
        }

        return curve;
    }
}
