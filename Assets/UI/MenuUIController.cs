using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public AudioDetector audioDetector;
    public GameObject gameManager; // Optional if you have central controller

    public void StartGame()
    {
        Debug.Log("Start Game Pressed");
        // Add your game-starting logic here
    }

    public void EndGame()
    {
        Debug.Log("End Game Pressed");
        Application.Quit(); // Or return to main menu, etc.
    }

    //public void SetMicrophoneSensitivity(float value)
    //{
    //    Debug.Log("Setting mic sensitivity: " + value);
    //    if (audioDetector != null)
    //    {
    //        // Example: map value to thresholds
    //        audioDetector.sampleWindow = Mathf.RoundToInt(Mathf.Lerp(64, 2048, value));
    //    }
    //}
}
