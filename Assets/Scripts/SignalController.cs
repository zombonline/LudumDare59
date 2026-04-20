using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignalController : MonoBehaviour
{
    [Serializable]
    public class SignalBinding
    {
        public string label;                        // matches SignalRequirement.label
        public MonoBehaviour source;
        public TextMeshProUGUI feedbackText;
        public MonoBehaviour signalDisplay;

        [NonSerialized] public ISignalSource SignalSource;
        [NonSerialized] public ISignalDisplay SignalDisplay;
        [NonSerialized] public SignalRequirement Requirement; // assigned at runtime
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
            binding.SignalDisplay = binding.signalDisplay as ISignalDisplay;

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
        int activeCount = 0;

        foreach (var binding in bindings)
        {
            if (binding.SignalSource == null || binding.Requirement == null) continue;

            float score = binding.Requirement.Evaluate(binding.SignalSource, AudioController.Instance.GetMessageProgress());
            total += score;
            activeCount++;
            binding.feedbackText.text = binding.Requirement.label + ": " + score + "/1\n";
            binding.SignalDisplay?.UpdateDisplay(score);
        }

        SignalQuality = activeCount > 0 ? total / activeCount : 0f;
        outputText.text = "SignalQuality: " + SignalQuality;
    }
    
    /// <summary>Matches requirements from the given message to bindings by label.</summary>
    public void LoadRequirements(Message message)
    {
        foreach (var binding in bindings)
            binding.Requirement = null;

        foreach (var req in message.requirements)
        {
            SignalBinding binding = bindings.Find(b => b.label == req.label);
            if (binding != null)
                binding.Requirement = req;
            else
                Debug.LogWarning($"[SignalController] No binding with label '{req.label}'.");
        }
    }

    
}