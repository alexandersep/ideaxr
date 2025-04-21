using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SensitivityLabel : MonoBehaviour
{
    public TMP_Text valueText;
    public Slider sensitivitySlider;

    void Update()
    {
        valueText.text = "" + (Mathf.RoundToInt(sensitivitySlider.value * 200).ToString());
    }
}
