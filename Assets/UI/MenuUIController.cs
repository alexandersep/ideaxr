using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public AudioDetector audioDetector;
    public GameObject gameManager; // Optional if you have central controller
    public GameObject gameplayUI;

    public InvigilatorAI invigilatorAI; // Direct reference to the script

    public void StartGame()
    {
        invigilatorAI.isActive = true;
        Debug.Log("Invigilator AI is now active.");

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false); // Hide the UI
        }
    }

    public void EndGame()
    {
        Debug.Log("End Game Pressed");
        Application.Quit(); // Or return to main menu, etc.
    }
}
