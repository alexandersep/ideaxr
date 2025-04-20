using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SensitivityLabel : MonoBehaviour
{
    public TMP_Text valueText;
    public Slider sensitivitySlider;

    void Update()
    {
        valueText.text = "Sens.: " + Mathf.RoundToInt(sensitivitySlider.value).ToString();
    }
}
