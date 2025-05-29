
using System.Collections.Generic;

public class TalkDataMemory
{
    /// <summary>
    /// LLMの会話履歴を保持するリスト
    /// </summary>
    private List<SingleTalkData> _talkMemoryList = new();

    /// <summary>
    /// 会話データを追加するメソッド
    /// </summary>
    /// <param name="question">質問内容</param>
    /// <param name="answer">回答</param>
    public void AddTalkData(string question, string answer)
    {
   
        // 質問と回答のデータを追加
        _talkMemoryList.Add(new SingleTalkData(question, answer));
    }

    public string GetTalkData()
    {
        if (_talkMemoryList.Count <= 0)
        {
            return null;
        }

        // 会話履歴を文字列に変換
        string talkData = "--進行履歴--";
        foreach (SingleTalkData data in _talkMemoryList)
        {
            talkData += $"プレイヤーの選択: {data.Question}\n結果: {data.Answer}\n";
        }
        return talkData;
    }

    /// <summary>
    /// 会話データをリセットするメソッド
    /// </summary>
    public void ResetTalkData()
    {
        // 会話履歴をリセット
        _talkMemoryList.Clear();
    }
}

public class SingleTalkData
{ 
    /// <summary>
    /// 質問内容
    /// </summary>
    private string _question;
    public string Question => _question;

    /// <summary>
    /// 回答内容
    /// </summary>
    private string _answer;
    public string Answer => _answer;

    public SingleTalkData(string question, string answer)
    {
        _question = question;
        _answer = answer;
    }
}
