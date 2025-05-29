
using System.Collections.Generic;

public class TalkDataMemory
{
    /// <summary>
    /// LLM�̉�b������ێ����郊�X�g
    /// </summary>
    private List<SingleTalkData> _talkMemoryList = new();

    /// <summary>
    /// ��b�f�[�^��ǉ����郁�\�b�h
    /// </summary>
    /// <param name="question">������e</param>
    /// <param name="answer">��</param>
    public void AddTalkData(string question, string answer)
    {
   
        // ����Ɖ񓚂̃f�[�^��ǉ�
        _talkMemoryList.Add(new SingleTalkData(question, answer));
    }

    public string GetTalkData()
    {
        if (_talkMemoryList.Count <= 0)
        {
            return null;
        }

        // ��b�����𕶎���ɕϊ�
        string talkData = "--�i�s����--";
        foreach (SingleTalkData data in _talkMemoryList)
        {
            talkData += $"�v���C���[�̑I��: {data.Question}\n����: {data.Answer}\n";
        }
        return talkData;
    }

    /// <summary>
    /// ��b�f�[�^�����Z�b�g���郁�\�b�h
    /// </summary>
    public void ResetTalkData()
    {
        // ��b���������Z�b�g
        _talkMemoryList.Clear();
    }
}

public class SingleTalkData
{ 
    /// <summary>
    /// ������e
    /// </summary>
    private string _question;
    public string Question => _question;

    /// <summary>
    /// �񓚓��e
    /// </summary>
    private string _answer;
    public string Answer => _answer;

    public SingleTalkData(string question, string answer)
    {
        _question = question;
        _answer = answer;
    }
}
