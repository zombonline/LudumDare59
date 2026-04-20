// Assets/Scripts/IntroController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroController : MonoBehaviour
{
    [SerializeField] private float timer;
    
    
    private void Update()
    {
        timer -= Time.deltaTime;
        // Any key / click to start
        if (Keyboard.current.anyKey.wasPressedThisFrame || 
            Mouse.current.leftButton.wasPressedThisFrame && timer <= 0)
            SceneLoader.LoadGame();
    }
}