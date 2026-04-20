using System;
using UnityEngine;

/// <summary>
/// Local fallback evaluation used when the Gemini API is unavailable.
/// Scores the player's transcription by checking how many of the message's
/// keywords are present, then outputs a canned officer response.
/// </summary>
public class FallbackEvaluator : MonoBehaviour
{
    private const float CorrectThreshold = 0.85f;
    private const float MinorThreshold   = 0.5f;

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
        "Good lord, man. That was the transmission? I weep for the regiment.",
        "I see. You've turned a straightforward order into something quite catastrophic. Inspiring, in a bleak sort of way.",
        "Remarkable. You've managed to get almost nothing right. There's a talent in that, I suppose."
    };

    /// <summary>
    /// Scores the player's transcription against the message's keywords and
    /// invokes the callback with a canned officer response.
    /// </summary>
    public void Evaluate(Message message, string playerText, Action<string> onComplete)
    {
        float score = ComputeKeywordScore(message.keywords, playerText);
        onComplete?.Invoke(PickResponse(score));
    }

    private static float ComputeKeywordScore(string[] keywords, string playerText)
    {
        if (keywords == null || keywords.Length == 0) return 1f;

        int hits = 0;
        foreach (string keyword in keywords)
        {
            if (string.IsNullOrWhiteSpace(keyword)) continue;
            if (playerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                hits++;
        }

        return (float)hits / keywords.Length;
    }

    private static string PickResponse(float score)
    {
        string[] pool = score >= CorrectThreshold ? CorrectResponses
                      : score >= MinorThreshold   ? MinorResponses
                                                  : MajorResponses;
        return pool[UnityEngine.Random.Range(0, pool.Length)];
    }
}

