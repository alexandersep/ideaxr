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
}
