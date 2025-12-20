using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PitchDetector : MonoBehaviour
{
    public int sampleRate = 480000;
    public float fmin = 80.0f;
    public float fmax = 1000.0f;
    
    public float LastF0Hz { get; private set; }
    public float LastMidi { get; private set; }
    public float Confidence { get; private set; }

    public void Analyze(float[] frame)
    {
        LastF0Hz = 0.0f;
        LastMidi = 0.0f;
        Confidence = 0.0f;
        
        if (frame == null || frame.Length < 64) return;

        float f0;
        float conf;

        if (!EstimatePitchAutoCorrelation(frame, sampleRate, fmin, fmax, out f0, out conf))
        {
            return;
        }

        LastF0Hz = f0;
        Confidence = conf;

        if (f0 > 0.0f)
        {
            LastMidi = HzToMidi(f0);
        }
    }

    public void Reset()
    {
        LastF0Hz = 0f;
        LastMidi = 0f;
        Confidence = 0f;
    }

    float HzToMidi(float hz)
    {
        return 69.0f + 12.0f * (Mathf.Log(hz / 440.0f) / Mathf.Log(2.0f));
    }

    bool EstimatePitchAutoCorrelation(float[] x, int sr, float minHz, float maxHz, out float f0, out float conf)
    {
        f0 = 0.0f;
        conf = 0.0f;

        int minLag = Mathf.FloorToInt((float)sr / maxHz);
        int maxLag = Mathf.FloorToInt((float)sr / minHz);

        if (maxLag >= x.Length - 1) return false;
        double energy = 0.0;
        int i;
        for (i = 0; i < x.Length; i++)
        {
            energy += (double)(x[i] * x[i]);
        }

        if (energy < 1e-6) return false;

        float best = -1.0f;
        int bestLag = -1;

        int lag;
        for (lag = minLag; lag <= maxLag; lag++)
        {
            double sum = 0.0;
            for (i = 0; i < x.Length - lag; i++)
            {
                sum += (double)(x[i] * x[i + lag]);
            }

            float corr = (float)(sum / energy);
            if (corr > best)
            {
                best = corr;
                bestLag = lag;
            }
        }

        if (bestLag <= 0) return false;

        f0 = (float)sr / (float)bestLag;
        conf = Mathf.Clamp01(best);
        if (conf < 0.10f) return false;

        return true;
    }
}
