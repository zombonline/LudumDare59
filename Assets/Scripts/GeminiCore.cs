using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using TMPro; 

public class GeminiCore : MonoBehaviour
{
    /// <summary>Fired when the AI response has been received and displayed.</summary>
    public event Action OnResponseReady;
    /// <summary>Fired with the evaluated severity: "correct", "minor", or "major".</summary>
    public event Action<string> OnSeverityEvaluated;
    [Header("API Configuration")]
    private string apiKey;

    [SerializeField] private TextAsset apiKeyTextFile;

    private string url =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key=";
    [Header("UI References")]
    public TextInput textInput;
    public TMP_Text outputText;

    [Header("Fallback")]
    [SerializeField] private FallbackEvaluator fallbackEvaluator;
    [SerializeField] private int requestTimeoutSeconds = 8;

    private void Awake()
    {
        LoadApiKey();
    }

    void Start()
    {
        textInput.onEnterPressed += OnEnterPressed;
    }

    private void OnDestroy()
    {
        textInput.onEnterPressed -= OnEnterPressed;
    }

    private void OnEnterPressed()
    {
        string original = GameStateManager.Instance.CurrentMessage.message;
        string player = textInput.Text;

        if (!string.IsNullOrEmpty(player))
        {
            string prompt = BuildPrompt(original, player);
            StartCoroutine(SendPrompt(prompt, original, player));

            outputText.text = "Evaluating...";
        }
    }
    void LoadApiKey()
    {
        apiKey = apiKeyTextFile.text;
    }
    

    public IEnumerator SendPrompt(string userPrompt, string original, string playerText)
    {
        GeminiRequest requestData = new GeminiRequest();
        requestData.contents = new Content[] {
            new Content {
                parts = new Part[] {
                    new Part { text = userPrompt }

                }
            }
        };
        
        requestData.generationConfig = new GenerationConfig
        {
            temperature = 0.2f
        };

        string jsonPayload = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(url + apiKey, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = requestTimeoutSeconds;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Full Error Report: " + request.downloadHandler.text);
                RunFallback(original, playerText);
            }
            else
            {
                GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(request.downloadHandler.text);
                
                if (response.candidates != null && response.candidates.Length > 0)
                {
                    string raw = response.candidates[0].content.parts[0].text;
                    Debug.Log("RAW AI OUTPUT: " + raw);

                    string cleaned = raw.Replace("```json", "").Replace("```", "").Trim();

                    try
                    {
                        EvaluationResult result = JsonUtility.FromJson<EvaluationResult>(cleaned);

                        if (result != null && !string.IsNullOrEmpty(result.response))
                        {
                            outputText.text = result.response;
                            OnSeverityEvaluated?.Invoke(result.severity);
                        }
                        else
                        {
                            outputText.text = "Invalid response format.";
                            OnSeverityEvaluated?.Invoke("minor");
                        }
                        OnResponseReady?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("JSON Parse Error: " + e.Message);
                        outputText.text = "Failed to parse AI response.";
                        OnResponseReady?.Invoke();
                    }
                }
                else
                {
                    outputText.text = "No response received.";
                    OnResponseReady?.Invoke();
                }
            }
        }
    }

    //JSON Utility Classes
    [Serializable]
    public class GeminiRequest
    {
        public Content[] contents;
        public GenerationConfig generationConfig;
    }

    [Serializable]
    public class Content
    {
        public Part[] parts;
    }

    [Serializable]
    public class Part
    {
        public string text;
    }

    [Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
    }
    [Serializable]
    public class GenerationConfig
    {
        public float temperature;
    }
    [Serializable]
    public class Candidate
    {
        public ResponseContent content;
    }

    [Serializable]
    public class ResponseContent
    {
        public Part[] parts;
    }
    [Serializable]
    public class EvaluationResult
    {
        public string severity;
        public string[] differences;
        public string consequence;
        public string response;
    }
    private void RunFallback(string original, string playerText)
    {
        if (fallbackEvaluator == null)
        {
            Debug.LogWarning("[GeminiCore] No FallbackEvaluator assigned.");
            OnResponseReady?.Invoke();
            return;
        }

        Message message = GameStateManager.Instance.CurrentMessage;
        fallbackEvaluator.Evaluate(message, playerText, (response, severity) =>
        {
            outputText.text = response;
            OnSeverityEvaluated?.Invoke(severity);
            OnResponseReady?.Invoke();
        });
    }

    private string BuildPrompt(string original, string player)
    {
        return $@"
        You are evaluating a misheard radio transmission in a grounded, slightly tense setting.

        ORIGINAL:
        {original}

        PLAYER:
        {player}

        TASK:
        1. Identify key differences in meaning.
        2. Classify severity as one of:
           - correct
           - minor
           - major
        3. Describe a short consequence (max 2 sentences).
        4. Provide a short response from the player's direct superior.

        CHARACTER:
        An eccentric, ageing British WW2 officer. Slightly unhinged but competent. 
        Speaks with dry, dark humour. Treats serious situations with inappropriate levity. You are Sergeant Jenkins.
        If severity is ""major"": the officer informs the player they are relieved of their post and face a court martial. 
        Still dry and darkly humorous, but with genuine finality — no room for redemption.

        TONE RULES:
        - British phrasing and vocabulary (""good lord"", ""man"", ""bloody"", ""splendid mess"")
        - Dry sarcasm, understatement, or ironic praise
        - Dark humour about danger, but never cartoonish
        - Mildly scolding, but not angry
        - Sounds like he has seen too much war and is unfazed
        - Think Blackadder, Dad's Army.

        STYLE CONSTRAINTS:
        - 1–2 sentences only
        - No modern slang
        - No emojis
        - No shouting or excessive punctuation
        - Keep it grounded in the situation

        EXAMPLES OF VOICE (do not copy, just match tone):
        - ""Splendid. You've managed to turn a simple order into a minor catastrophe.""
        - ""Ah yes, charging in when told not to—bold, if somewhat suicidal.""
        - ""Not quite what I had in mind, but I admire the confidence.""

        The response MUST directly reflect the mistake and its consequence.

        RULES:
        - Only use information present in the two texts
        - Do not invent new facts
        - Keep tone grounded and concise
        - No extra commentary
        - The ""response"" must reference the consequence implicitly or explicitly
        - Do not repeat the original or player text

        OUTPUT (JSON ONLY):
        {{
          ""severity"": ""correct|minor|major"",
          ""differences"": [""...""],
          ""consequence"": ""..."",
          ""response"": ""...""
        }}
        ";
    }
}