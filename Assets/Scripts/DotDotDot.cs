using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class DotDotDot : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmpro;
    private string originalText;
    private int dotCount = 0;
    [SerializeField] private float delay;

    private void Start()
    {
        if (tmpro == null)
        {
            Debug.LogWarning("[DotDotDot] No TextMeshPro assigned.", this);
            return;
        }
        originalText = tmpro.text;
        StartCoroutine(UpdateText());
    }

    private IEnumerator UpdateText()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            dotCount++;
            dotCount = dotCount % 4;
            tmpro.text = originalText + String.Concat(Enumerable.Repeat(".", dotCount));
        }
    }
}
