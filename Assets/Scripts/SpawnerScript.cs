using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField, Header("��������u���b�N")]
    private BlockScript[] _blocks = default;
    [SerializeField,Header("�u���b�N���o�Ă��鏇��")]
    private int[] _blooksLen = default;
    //_blooksLen�̃C���f�b�N�X�l
    private int _lenPoint = default;
    //�l�N�X�g��\������������
    private BlockScript[] _saveBloocks = new BlockScript[3];

    //�~�m�̈ʒu����
    private Vector3 _minoBlockRevision = new Vector3(0.5f, 0.5f, 0);

    //�l�N�X�g�~�m��\��������ʒu
    private Vector3 _nestFastPosition = new Vector3(10, -6, 0);
    private Vector3 _nestSecondPosition = new Vector3(10, -10, 0);
    private Vector3 _nestThirdPosition = new Vector3(10, -14, 0);


    private void Awake()
    {
        //�ŏ��̒l���������_���Ō��߂�
        _lenPoint = Random.Range(0, _blooksLen.Length);
    }
    /// <summary>
    /// �u���b�N��I��
    /// </summary>
    /// <returns></returns>
    private BlockScript GetRandomBlock()
    {
        //���g�������Ă��邩
        if (_blocks[_blooksLen[_lenPoint]])
        {
            //lenPoint��1�Â��₷
            _lenPoint++;
            //�z��̍Ō�ɂȂ�����ŏ��ɖ߂�
            if (_lenPoint > _blooksLen.Length - 1)
            {
                _lenPoint = 0;
            }
            //��������u���b�N�n��
            return _blocks[_blooksLen[_lenPoint]];
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// �I�΂ꂽ�u���b�N�𐶐�
    /// </summary>
    /// <returns></returns>
    public BlockScript SpwnBlock()
    {
        //�I�΂ꂽ�u���b�N�𐶐�
        BlockScript block = Instantiate(GetRandomBlock(), transform.position, Quaternion.identity);
        //I�~�m�u���b�N�����ʒu����
        if (block.GetISpin)
        {
            block.transform.position += _minoBlockRevision;
        }
        //�l�N�X�g���X�V����
        NextBlock();

        if (block)
        {
            return block;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// �l�N�X�g�̃u���b�N��\��
    /// </summary>
    /// <returns></returns>
    private BlockScript[] NextBlock()
    {
        //�l�N�X�g������
        for (int j = 0; j < _saveBloocks.Length; j++)
        {
            if (_saveBloocks[j] != null)
            {
                Destroy(_saveBloocks[j].gameObject);
            }
        }
        //1��̃u���b�N��\��������
        int i = 0;
        //�z��𒴂���ꍇ�ŏ��ɖ߂�
        if (_lenPoint + 1 < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + 1];
        }
        //1�Ԗڃu���b�N�𐶐�
        _saveBloocks[0] = Instantiate(_blocks[i], transform.position + _nestFastPosition, Quaternion.identity);
        //�z��𒴂���ꍇ�ŏ��ɖ߂�
        if (_lenPoint + 2 < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + 2];
        }
        //2���\��
        else
        {
            i = _lenPoint + 2 - _blooksLen.Length;
        }
        //2�Ԗڃu���b�N�𐶐�
        _saveBloocks[1] = Instantiate(_blocks[i], transform.position + _nestSecondPosition, Quaternion.identity);

        //�z��𒴂���ꍇ�ŏ��ɖ߂�
        if (_lenPoint + 3 < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + 3];
        }
        //3���\��
        else
        {
            i = _lenPoint + 3 - _blooksLen.Length;
        }
        //3�Ԗڃu���b�N�𐶐�
        _saveBloocks[2] = Instantiate(_blocks[i], transform.position + _nestThirdPosition, Quaternion.identity);

        //���ׂē����Ă���return����
        if (_saveBloocks[0] != null && _saveBloocks[1] != null && _saveBloocks[2] != null)
        {
            return _saveBloocks;
        }
        else
        {
            return null;
        }
    }
}
