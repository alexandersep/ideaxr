using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public AudioDetector audioDetector;
    public GameObject gameplayUI;
    public GameObject passedText;
    public GameObject failedText;
    public GameObject caughtText;
    public GameResultChecker resultChecker;
    public InvigilatorAI invigilatorAI; // Direct reference to the script

    private Vector3 aiStartPosition;
    private Quaternion aiStartRotation;
    private bool hasDisplayedFail = false;

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

        hasDisplayedFail = false;

        // Reset AI position and pause it
        invigilatorAI.transform.position = aiStartPosition;
        invigilatorAI.transform.rotation = aiStartRotation;
        invigilatorAI.agent.ResetPath();
        invigilatorAI.isActive = false;
        invigilatorAI.strikes = 0;

        // Hide result texts
        passedText.SetActive(false);
        failedText.SetActive(false);
        caughtText.SetActive(false);

        // Reset game state
        gameplayUI.SetActive(true);
        resultChecker.ResetResult();
    }

    public void StartGame()
    {
        Debug.Log("Invigilator AI is now active.");
        invigilatorAI.isActive = true;
        gameplayUI.SetActive(false);
    }

    public void ShowFailureFromCatch()
    {
        if (hasDisplayedFail) return;

        Debug.Log("Showing fail screen from Invigilator catch.");
        hasDisplayedFail = true;

        if (gameplayUI != null) gameplayUI.SetActive(true);

        //if (failedText != null) failedText.SetActive(true);
        if (caughtText != null)
        {
            caughtText.SetActive(true); // Show the caught-specific message
        }
        else
        {
            failedText?.SetActive(true); // fallback if caughtText is not set
        }

        if (invigilatorAI != null) invigilatorAI.isActive = false;
    }

    public void EndGame()
    {
        Debug.Log("End Game Pressed");
        Application.Quit(); // Or return to main menu, etc.
    }
}
