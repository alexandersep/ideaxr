using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Source: https://www.youtube.com/watch?v=GAHMreCT4SY
public class AudioDetector : MonoBehaviour
{
    public int sampleWindow = 64;
    public int microphoneIndex = 0;

    private AudioClip _microphoneClip;
    private string _microphoneName;

    // Start is called before the first frame update
    private void Start()
    {
        MicrophoneToAudioClip(microphoneIndex);
    }

    private void MicrophoneToAudioClip(int microphoneIndex)
    {
        foreach (var name in Microphone.devices)
        {
            Debug.Log(name);
        }

        _microphoneName = Microphone.devices[microphoneIndex];

        _microphoneClip = Microphone.Start(_microphoneName, true, 20, AudioSettings.outputSampleRate);
    }

    public float GetAudioFromMicrophone()
    {
        return GetAudioFromClip(Microphone.GetPosition(_microphoneName), _microphoneClip);
    }
    public float GetAudioFromClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;

        if (startPosition < 0)
        {
            return 0;
        }
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);


        float totalLoudness = 0;

        foreach (float sample in waveData)
        {
            totalLoudness += Mathf.Abs(sample);
        }

        return totalLoudness / sampleWindow;
    }
}
