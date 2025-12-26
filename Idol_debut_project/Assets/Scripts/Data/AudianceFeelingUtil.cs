using Data;
using UnityEngine;

public class AudianceFeelingUtil
{
    public static AudianceData.EAudianceFeeling FromScore100(int score100)
    {
        score100 = Mathf.Clamp(score100, 0, 100);
        if (score100 < 10)
        {
            return AudianceData.EAudianceFeeling.Bad;
        } else if (score100 < 40)
        {
            return AudianceData.EAudianceFeeling.Huh;
        }
        else if (score100 < 60)
        {
            return AudianceData.EAudianceFeeling.SoSo;
        } else if (score100 < 90)
        {
            return AudianceData.EAudianceFeeling.Good;
        }
        else
        {
            return AudianceData.EAudianceFeeling.Perfect;
        }
    }
}
