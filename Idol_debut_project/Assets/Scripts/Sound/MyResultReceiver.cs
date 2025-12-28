using System;
using UnityEngine;

public class MyResultReceiver : MonoBehaviour
{
    [SerializeField] private VocalJudge judge;

    public event Action<VocalResult> OnResultReady; 
    void OnEnable()
    {
        judge.OnFinished += HandleFinished;
    }

    void OnDisable()
    {
        judge.OnFinished -= HandleFinished;
    }

    void HandleFinished(VocalResult r)
    {
        Debug.Log("최종 점수 : " + r.finalScore100);
        Debug.Log("Feeling : " + r.feeling);
        OnResultReady?.Invoke(r);
    }
}
