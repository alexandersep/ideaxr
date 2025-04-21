using UnityEngine;
using UnityEngine.UI;

public class GameResultChecker : MonoBehaviour
{
    public HandRaise leftHand;
    public HandRaise rightHand;
    public QuestionsChecker questionsChecker;

    public GameObject passedText;
    public GameObject failedText;

    public GameObject gameplayUI; // Canvas or panel to reactivate

    private bool hasDisplayedResult = false;

    void Update()
    {
        if (!hasDisplayedResult)
        {
            bool isHandRaised = leftHand._isHandRaised || rightHand._isHandRaised;
            float result = questionsChecker._result;

            if (isHandRaised)
            {
                if (result >= 0.9f)
                {
                    passedText.SetActive(true);
                    Debug.Log("Player passed the exam!");
                }
                else
                {
                    failedText.SetActive(true);
                    Debug.Log("Player failed the exam.");
                }

                if (gameplayUI != null)
                {
                    gameplayUI.SetActive(true); // Show the UI again
                }

                hasDisplayedResult = true;
            }
        }
    }

    public void ResetResult()
    {
        hasDisplayedResult = false;
        passedText.SetActive(false);
        failedText.SetActive(false);
    }
}
