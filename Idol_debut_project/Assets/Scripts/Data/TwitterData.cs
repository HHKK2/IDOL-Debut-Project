using UnityEngine.Serialization;

namespace Data
{
    [System.Serializable]
    public struct TwitterData
    {
        public string ID;
        public AudianceData.EAudianceFeeling feeling;
        public int reputationStatus;
        public string profilePic;
        public string name;
        public string userID;
        public string text;
    }
    [System.Serializable]
    public struct TwitterDataList
    {
        public TwitterData[] twitter;
    }
}