using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField, Header("��������u���b�N")]
    private BlockScript[] _blocks = default;
    [SerializeField, Header("�u���b�N���o�Ă��鏇��")]
    private int[] _blooksLen = default;
    //_blooksLen�̃C���f�b�N�X�l
    private int _lenPoint = default;
    //�l�N�X�g��\������������
    private BlockScript[] _saveBloocks = new BlockScript[3];

    //I�~�m�̈ʒu����
    private Vector3 _minoBlockRevision = new Vector3(0.5f,0.5f, 0);

    //�l�N�X�g�~�m��\�������邻�ꂼ��̈ʒu
    private Vector3 _nestFastPosition = new Vector3(10, -6, 0);
    private Vector3 _nestSecondPosition = new Vector3(10, -10, 0);
    private Vector3 _nestThirdPosition = new Vector3(10, -14, 0);

    //�l�N�X�g�~�m�����Ԑ悩���Z����l
    private const int _fastIndex = 1;
    private const int _secondIndex = 2;
    private const int _thirdIndex = 3;
    //�l�N�X�g�u���b�N��ۑ�����z��̒l
    private int _saveIndex = 0;

    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        //�ŏ��̒l���������_���Ō��߂�
        _lenPoint = Random.Range(0, _blooksLen.Length);
    }
    /// <summary>
    /// <para>�u���b�N��I��</para>
    /// </summary>
    /// <returns>BlockScript</returns>
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
    /// <para>�I�΂ꂽ�u���b�N�𐶐�</para>
    /// </summary>
    /// <returns>BlockScript</returns>
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

        //��������Ă�����n��
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
    /// <para>�l�N�X�g�̃u���b�N��\������</para>
    /// </summary>
    /// <returns>BlockScript[]</returns>
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
        //1��̃u���b�N�����肷��index�l
        int i = 0;
        //�z��𒴂���ꍇ�ŏ��ɖ߂�
        if (_lenPoint + _fastIndex < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + _fastIndex];
        }
        //�u���b�N�̃z�[���h��ۑ�����l��������
        _saveIndex = 0;
        //1�Ԗڃu���b�N�𐶐�
        _saveBloocks[_saveIndex] = Instantiate(_blocks[i], transform.position + _nestFastPosition, Quaternion.identity);

        //�J�E���g�A�b�v
        _saveIndex++;

        //�z��𒴂���ꍇ�ŏ��ɖ߂�
        if (_lenPoint + _secondIndex < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + _secondIndex];
        }
        //2���\��
        else
        {
            i = _lenPoint + _secondIndex - _blooksLen.Length;
        }
        //2�Ԗڃu���b�N�𐶐�
        _saveBloocks[_saveIndex] = Instantiate(_blocks[i], transform.position + _nestSecondPosition, Quaternion.identity);

        //�J�E���g�A�b�v
        _saveIndex++;

        //�z��𒴂���ꍇ�ŏ��ɖ߂�
        if (_lenPoint + _thirdIndex < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + _thirdIndex];
        }
        //3���\��
        else
        {
            i = _lenPoint + _thirdIndex - _blooksLen.Length;
        }
        //3�Ԗڃu���b�N�𐶐�
        _saveBloocks[_saveIndex] = Instantiate(_blocks[i], transform.position + _nestThirdPosition, Quaternion.identity);

        //�J�E���g�A�b�v
        _saveIndex++;

        //�z�񂪂��ׂē����Ă��邩�m�F����
        for (int K = 0; K < _saveIndex; K++)
        {
            //�����Ă��玟�̒l��
            if (_saveBloocks[K] != null)
            {
                continue;
            }
            else
            {
                //�Ȃ�������null��Ԃ�
                return null;
            }
        }
        //BlockScript�̔z���n��
        return _saveBloocks;
    }
}
