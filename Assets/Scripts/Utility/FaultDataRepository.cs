using UnityEngine;

public class FaultDataRepository
{
    private string[] _faultData;

    public FaultDataRepository()
    {
        _faultData = new string[]
        {
           "���R�ЊQ�n�i�n�k�A�΍ЁA�^���A����Ȃǁj",
              "�����E�o�n�i�G���x�[�^�[�A�n�����A�ԓ��Ȃǁj",
              "�T�o�C�o���n�i���l���A�R�x����A�����Ȃǁj",
              "�Љ�I��@�n�i��d�A��ʖ�ჁA�V�X�e����Q�Ȃǁj",
              "�S���I��@�n�i�L���r���A���Ԑ����A�I���̃W�����}�Ȃǁj",
              "�l�Ԋ֌W�n�i�l���A���؂�A�M���̊�@�Ȃǁj",
              "�Z�p�I��@�n�i�@�B�̏�A�T�C�o�[�U���A�E�C���X�����Ȃǁj",
              "����@�n�i�����A�����͊��A���Ԍn�̕���Ȃǁj",
              "��Ìn�i�����ǁA�򕨒��ŁA�ً}��p�Ȃǁj",
              "�ƍߌn�i�U���A�����A���\�Ȃǁj"
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