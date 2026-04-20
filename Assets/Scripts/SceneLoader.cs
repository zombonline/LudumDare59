using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private const string IntroScene = "IntroScene";
    private const string GameScene  = "SampleScene";

    /// <summary>Loads the intro / game over screen.</summary>
    public static void LoadIntro() => SceneManager.LoadScene(IntroScene);

    /// <summary>Starts a fresh game session.</summary>
    public static void LoadGame()  => SceneManager.LoadScene(GameScene);
}