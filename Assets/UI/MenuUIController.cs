using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public GameObject Exam;
    public AudioDetector audioDetector;
    public GameObject gameplayUI;
    public GameObject EndOfGame;
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
        Exam.SetActive(false);
        HideText();
    }

    public void RestartGame()
    {
        Debug.Log("Starting game...");

        hasDisplayedFail = false;

        ResetInvigilator();
        HideText();

        // Reset game state
        gameplayUI.SetActive(false);
        resultChecker.ResetResult();

        // Reset Exam to be invisible
        Exam.SetActive(true);
    }

    private void ResetInvigilator()
    {
        invigilatorAI.transform.position = aiStartPosition;
        invigilatorAI.transform.rotation = aiStartRotation;
        invigilatorAI.agent.ResetPath();
        invigilatorAI.isActive = true;
        invigilatorAI.strikes = 0;
    }

    private void HideText()
    {
        passedText.SetActive(false);
        failedText.SetActive(false);
        caughtText.SetActive(false);
    }

    public void StartGame()
    {
        RestartGame();
    }

    public void ShowFailure()
    {
        if (hasDisplayedFail) return;

        Debug.Log("Showing fail screen from Invigilator catch.");
        hasDisplayedFail = true;

        gameplayUI.SetActive(true);

        //if (failedText != null) failedText.SetActive(true);
        if (caughtText != null)
        {
            caughtText.SetActive(true); // Show the caught-specific message
        }
        else
        {
            failedText?.SetActive(true); // fallback if caughtText is not set
        }
        Exam.SetActive(false);


        if (invigilatorAI != null)
        {
            invigilatorAI.isActive = false;
        }
    }

    public void EndGame()
    {
        Debug.Log("End Game Pressed");
        Application.Quit(); // Or return to main menu, etc.
    }
}
