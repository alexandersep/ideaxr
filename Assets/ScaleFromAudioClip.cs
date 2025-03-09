using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFromAudioClip : MonoBehaviour
{
    public AudioSource source;
    public Vector3 minScale, maxScale;
    public AudioDetector detector;

    public float loudnessSensibility = 100f;
    public float threshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float loudness = detector.GetAudioFromClip(source.timeSamples, source.clip) * loudnessSensibility;
        Debug.Log(loudness);
        if (loudness < threshold)
        {
            loudness = 0;
        }

        transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
    }
}
