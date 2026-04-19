using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignalController : MonoBehaviour
{
    [Serializable]
    public class SignalBinding
    {
        public MonoBehaviour source;
        public SignalRequirement requirement;
        public TextMeshProUGUI feedbackText;
        [NonSerialized]
        public ISignalSource SignalSource;
    }

    [SerializeField] private List<SignalBinding> bindings = new();
    [SerializeField] private TextMeshProUGUI outputText;

    public float SignalQuality { get; private set; }

    public IReadOnlyList<SignalBinding> Bindings => bindings;

    private void Awake()
    {
        foreach (var binding in bindings)
        {
            binding.SignalSource = binding.source as ISignalSource;

            if (binding.SignalSource == null)
                Debug.LogError($"[SignalController] {binding.source.name} does not implement ISignalSource.");
        }
    }
    
    private void Update()
    {
        if (bindings.Count == 0)
        {
            SignalQuality = 0f;
            return;
        }

        float total = 0f;

        foreach (var binding in bindings)
        {
            if (binding.SignalSource != null)
            {
                total += binding.requirement.Evaluate(binding.SignalSource.Value);
                binding.feedbackText.text = binding.requirement.label + ": " + binding.requirement.Evaluate(binding.SignalSource.Value) + "/1\n";
            }
        }
        SignalQuality = total / bindings.Count;
        outputText.text = "SignalQuality: " + SignalQuality;
    }
    
}