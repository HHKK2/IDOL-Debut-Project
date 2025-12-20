using UnityEngine;

public class VocalActivityDetector : MonoBehaviour
{
    public float rmsThreshold = 0.02f;
    public bool IsVocalActive { get; private set; }
    public float LastRms { get; private set; }

    public void Analyze(float[] frame)
    {
        if (frame == null || frame.Length == 0)
        {
            IsVocalActive = false;
            LastRms = 0.0f;
            return;
        }

        double sum = 0.0;
        int i;
        for (i = 0; i < frame.Length; i++)
        {
            float x = frame[i];
            sum += (double)(x * x);
        }

        double mean = sum / (double)frame.Length;
        float rms = Mathf.Sqrt((float)mean);

        LastRms = rms;
        IsVocalActive = (rms >= rmsThreshold);
    }
}
