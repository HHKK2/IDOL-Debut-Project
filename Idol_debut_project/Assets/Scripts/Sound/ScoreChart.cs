using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ScoreChart : MonoBehaviour
{
    [Serializable]
    public class Note
    {
        public float start;
        public float end;
        public int midi;
        public int tol_cents;
    }

    [Serializable]
    public class ChartData
    {
        public int version;
        public List<Note> notes;
    }

    public TextAsset scoreChartJson;
    public ChartData Chart { get; private set; }

    private void Awake()
    {
        LoadFromTextAsset();
    }

    public void LoadFromTextAsset()
    {
        if (scoreChartJson == null)
        {
            Debug.LogWarning("ScoreChart json is null");
            return;
        }

        Chart = JsonUtility.FromJson<ChartData>(scoreChartJson.text);
        if (Chart == null || Chart.notes == null)
        {
            Debug.LogError("Failed to parse score_chart,json");
        }
    }

    public Note GetActiveNote(float timeSec)
    {
        if (Chart == null || Chart.notes == null) return null;
        int i;
        for (i = 0; i < Chart.notes.Count; i++)
        {
            Note n = Chart.notes[i];
            if (n.start <= timeSec && timeSec < n.end)
            {
                return n;
            }
        }

        return null;
    }
}
