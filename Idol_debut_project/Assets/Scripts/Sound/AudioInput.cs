using System;
using UnityEngine;

public class AudioInput : MonoBehaviour
{
    [Header("Mic Settings")] public bool useMic = true;
    public string micDeviceName = null;
    public int sampleRate = 48000;

    [Header("Frame Settings")] public int frameSize = 2048;
    public float gain = 1.0f;

    private AudioClip micClip;
    private float[] frameBuffer;
    
    public bool IsReady { get; private set; }

    private void Awake()
    {
        frameBuffer = new float[frameSize];
        IsReady = false;
    }

    void Start()
    {
        if (useMic)
        {
            StartMic();
        }
        else
        {
            IsReady = true;
        }
    }

    void StartMic()
    {
        if (Microphone.devices == null || Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone device found");
            return;
        }

        if (micDeviceName == null || micDeviceName.Length == 0)
        {
            micDeviceName = Microphone.devices[0];
        }

        micClip = Microphone.Start(micDeviceName, true, 10, sampleRate);
        if (micClip == null)
        {
            Debug.LogError("Microphone.Start failed");
        }

        IsReady = true;
    }

    public bool TryGetFrame(out float[] buffer)
    {
        buffer = frameBuffer;
        if (!IsReady) return false;
        if (micClip == null) return false;

        int micPos = Microphone.GetPosition(micDeviceName);
        if (micPos < 0) return false;

        int startPos = micPos - frameSize;
        if (startPos < 0) startPos += micClip.samples;

        int samplesLeft = frameSize;
        int offset = 0;

        while (samplesLeft > 0)
        {
            int chunk = samplesLeft;
            int endPos = startPos + chunk;
            if (endPos > micClip.samples)
            {
                chunk = micClip.samples - startPos;
            }

            float[] temp = new float[chunk];
            micClip.GetData(temp, startPos);

            int i;
            for (i = 0; i < chunk; i++)
            {
                frameBuffer[offset + i] = temp[i] * gain;
            }

            offset += chunk;
            samplesLeft -= chunk;
            startPos = 0;
        }

        return true;
    }
}
