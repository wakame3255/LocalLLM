using UnityEngine;

public class FaultDataRepository
{
    private string[] _faultData;

    public FaultDataRepository()
    {
        _faultData = new string[]
        {
           "自然災害系（地震、火災、洪水、雪崩など）",
              "密室脱出系（エレベーター、地下室、車内など）",
              "サバイバル系（無人島、山岳遭難、砂漠など）",
              "社会的危機系（停電、交通麻痺、システム障害など）",
              "心理的危機系（記憶喪失、時間制限、選択のジレンマなど）",
              "人間関係系（人質、裏切り、信頼の危機など）",
              "技術的危機系（機械故障、サイバー攻撃、ウイルス感染など）",
              "環境危機系（汚染、資源枯渇、生態系の崩壊など）",
              "医療系（感染症、薬物中毒、緊急手術など）",
              "犯罪系（誘拐、強盗、詐欺など）"
        };
    }

    public string GetRandomFaultData()
    {
        if (_faultData.Length == 0)
        {
            return "No fault data available.";
        }
        int randomIndex = Random.Range(0, _faultData.Length);
        return _faultData[randomIndex];
    }
}