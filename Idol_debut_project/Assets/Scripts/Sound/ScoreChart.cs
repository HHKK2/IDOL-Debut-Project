using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreChart : MonoBehaviour
{
    [SerializeField] private TextAsset scoreChartJson;

    [Serializable]
    public class Note
    {
        public float start;
        public float end;
        public int midi;
        public int tol_cents;
    }

    [Serializable]
    public class Chart
    {
        public int version;
        public List<Note> notes;
        // segments도 json에 있지만 일단 여기선 notes만 쓸거라 생략 가능
    }

    private Chart chart;

    void Awake()
    {
        LoadChart();
    }

    private void LoadChart()
    {
        if (scoreChartJson == null)
        {
            Debug.LogError("[ScoreChart] scoreChartJson is null!");
            return;
        }

        chart = JsonUtility.FromJson<Chart>(scoreChartJson.text);

        if (chart == null || chart.notes == null)
        {
            Debug.LogError("[ScoreChart] Failed to parse chart json.");
            return;
        }

        Debug.Log("[ScoreChart] Loaded notes = " + chart.notes.Count);
    }

    private int curIdx = 0;
    // t초에 해당하는 노트가 있으면 반환, 없으면 null
    public Note GetNoteAtTime(float t)
    {
        if (chart == null || chart.notes == null || chart.notes.Count == 0) return null;

        if (curIdx >= chart.notes.Count) curIdx = chart.notes.Count - 1;
        if (curIdx < 0) curIdx = 0;

        while (curIdx > 0 && t < chart.notes[curIdx].start)
        {
            curIdx--;
        }

        while (curIdx < chart.notes.Count && t >= chart.notes[curIdx].end)
        {
            curIdx++;
        }
        
        if (curIdx >= 0 && curIdx < chart.notes.Count)
        {
            Note n = chart.notes[curIdx];
            if (t >= n.start && t < n.end) return n;
        }

        return null;
    }
}