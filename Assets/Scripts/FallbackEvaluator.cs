using System;
using UnityEngine;

/// <summary>
/// Local fallback evaluation used when the Gemini API is unavailable.
/// Scores the player's transcription by checking how many of the message's
/// keywords are present, then outputs a canned officer response.
/// </summary>
public class FallbackEvaluator : MonoBehaviour
{
    [Tooltip("Minimum keyword hits required for a 'correct' result.")]
    [SerializeField] private int correctHitsRequired = 2;
    [Tooltip("Minimum keyword hits required for a 'minor' result. Below this is 'major'.")]
    [SerializeField] private int minorHitsRequired = 1;

    private static readonly string[] CorrectResponses =
    {
        "Spot on, as expected. Carry on.",
        "Well, I'll be damned — you actually got it right. Proceed.",
        "Precisely correct. Don't let it go to your head."
    };

    private static readonly string[] MinorResponses =
    {
        "Close enough that I shan't have you shot. Well done, roughly.",
        "A few words astray, but the general drift was there. Acceptable.",
        "Nearly right, which in this war is almost the same as correct. Almost."
    };

    private static readonly string[] MajorResponses =
    {
        "That transmission was so catastrophically wrong that I have no choice but to relieve you of your post. You are to report for court martial at first light.",
        "I have seen men fail under pressure, but this is something else entirely. You are dismissed. Permanently. A court martial will handle the rest.",
        "Good lord, man. I don't know whether to pity you or have you shot. Court martial, eight o'clock sharp. Don't be late — it would only make things worse."
    };

    /// <summary>
    /// Scores the player's transcription against the message's keywords and
    /// invokes the callback with the response text and severity string.
    /// </summary>
    public void Evaluate(Message message, string playerText, Action<string, string> onComplete)
    {
        int hits = CountKeywordHits(message.keywords, playerText);
        string severity = hits >= correctHitsRequired ? "correct"
                        : hits >= minorHitsRequired   ? "minor"
                                                      : "major";
        onComplete?.Invoke(PickResponse(severity), severity);
    }

    private static int CountKeywordHits(string[] keywords, string playerText)
    {
        if (keywords == null || keywords.Length == 0) return int.MaxValue;

        int hits = 0;
        foreach (string keyword in keywords)
        {
            if (string.IsNullOrWhiteSpace(keyword)) continue;
            if (playerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                hits++;
        }
        return hits;
    }

    private static string PickResponse(string severity)
    {
        string[] pool = severity == "correct" ? CorrectResponses
                      : severity == "minor"   ? MinorResponses
                                              : MajorResponses;
        return pool[UnityEngine.Random.Range(0, pool.Length)];
    }
}

