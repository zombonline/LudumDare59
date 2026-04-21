// Assets/Scripts/IntroController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroController : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private GameObject continueText;
    
    private void Update()
    {
        timer -= Time.deltaTime;
        continueText.SetActive(timer <= 0);
        // Any key / click to start
        if (Keyboard.current.anyKey.wasPressedThisFrame || 
            Mouse.current.leftButton.wasPressedThisFrame && timer <= 0)
            SceneLoader.LoadGame();
    }
}