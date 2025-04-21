using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleFromMicrophone : MonoBehaviour
{
    public AudioSource source;
    public Vector3 minScale, maxScale;
    public Slider sensitivitySlider;
    public AudioDetector detector;

    public float minimumSensibility = 100f;
    public float maximumSensibility = 1000f;
    public float loudnessSensibility = 100f;
    public float threshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float loudness = detector.GetAudioFromMicrophone() * loudnessSensibility;
        //Debug.Log(loudness);
        if (loudness < threshold)
        {
            loudness = 0.01f;
        }

        if (sensitivitySlider == null) return;

        SetLoudnessSensibility(sensitivitySlider.value);

        transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
    }

    public void SetLoudnessSensibility(float value)
    {
        loudnessSensibility = Mathf.Lerp(minimumSensibility, maximumSensibility, value);
    }
}
