using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public AudioDetector audioDetector;
    public GameObject gameplayUI;
    public GameObject passedText;
    public GameObject failedText;
    public GameResultChecker resultChecker;
    public InvigilatorAI invigilatorAI; // Direct reference to the script

    private Vector3 aiStartPosition;
    private Quaternion aiStartRotation;

    void Start()
    {
        // Save original position/rotation of the AI
        if (invigilatorAI != null)
        {
            aiStartPosition = invigilatorAI.transform.position;
            aiStartRotation = invigilatorAI.transform.rotation;
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        // Reset AI position and pause it
        invigilatorAI.transform.position = aiStartPosition;
        invigilatorAI.transform.rotation = aiStartRotation;
        invigilatorAI.agent.ResetPath(); // stop any pathing
        invigilatorAI.isActive = false;

        // Hide result texts
        passedText.SetActive(false);
        failedText.SetActive(false);

        // Reset game state
        gameplayUI.SetActive(true);
        resultChecker.ResetResult();
    }

    public void StartGame()
    {
        //invigilatorAI.isActive = true;
        //Debug.Log("Invigilator AI is now active.");

        //if (gameplayUI != null)
        //{
        //    gameplayUI.SetActive(false); // Hide the UI
        //}

        invigilatorAI.isActive = true;
        gameplayUI.SetActive(false);
    }

    public void EndGame()
    {
        Debug.Log("End Game Pressed");
        Application.Quit(); // Or return to main menu, etc.
    }
}
