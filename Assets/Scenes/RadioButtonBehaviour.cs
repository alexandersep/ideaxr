using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadioButtonBehaviour : MonoBehaviour
{
    public Button[] _buttons;
    private Button _selectedButton = null;

    [SerializeField] private int _correctIndex; // Can be assigned in the inspector
    public bool _isCorrect;

    // Start is called before the first frame update
    private void Start()
    {
        _buttons = GetComponentsInChildren<Button>();
        foreach (Button b in _buttons)
        {
            b.onClick.AddListener(delegate { setButton(b); });
        }
    }

    private void Update()
    {
        colorButtonsCorrectly();
    }

    private void OnDestroy()
    {
        foreach (Button b in _buttons)
        {
            b.onClick.RemoveListener(delegate { setButton(b); });
        }
    }

    public bool getIsCorrect()
    {
        return _isCorrect;
    }

    private void setButton(Button button)
    {
        button.Select();
        _isCorrect = button == _buttons[_correctIndex] ? true : false;
        _selectedButton = button;
    }

    private void colorButtonsCorrectly()
    {
        foreach (Button b in _buttons)
        {
            ColorBlock colors = b.colors;
            colors.highlightedColor = new Color(0.5f, 0.5f, 0.5f, 0.3f); // gray with low alpha

            if (b == _selectedButton)
            {
                //if (_isCorrect)
                //{
                //    colors.normalColor = new Color(1, 0, 0, 0.3f);
                //    colors.selectedColor = new Color(1, 0, 0, 0.3f);
                //}
                //else
                //{
                colors.normalColor = new Color(0, 0, 1, 0.3f);
                colors.selectedColor = new Color(0, 0, 1, 0.3f);
                //}
            } else
            {
                colors.normalColor = new Color(0, 0, 0, 0);
                colors.selectedColor = new Color(0, 0, 0, 0);
            }
            b.colors = colors;
        }
    }
}
