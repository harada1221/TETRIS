//--------------------------------------
//�쐬��10/20
//�쐬�Ҍ��c
//
//�t�B�[���h�̊Ǘ�
//--------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScripts : MonoBehaviour
{
    //�t�B�[���h�̍���
    private const int _height = 30;
    //�t�B�[���h�̉�
    private int _width = 10;
    //�t�B�[���h�̍������
    private int _heightMax = 21;
    //�X�R�A�Ǘ��̃X�N���v�g
    private ScoreManagerScript _scoreManager = default;
    //�t�B�[���h�̔z��
    private int[,] _grid = default;
    //�u���b�N�̔z��
    private GameObject[] _minoBlocks = default;
    /// <summary>
    /// �����ݒ�
    /// </summary>
    private void Awake()
    {
        //�z�񐶐�
        _grid = new int[_width, _height];
        //ScoreManagerScript���擾
        _scoreManager = GameObject.FindObjectOfType<ScoreManagerScript>();
    }
    /// <summary>
    /// �Փ˔��������
    /// </summary>
    /// <param name="parentObj">���݂̃u���b�N</param>
    /// <returns>�Ԃ����Ă��邩</returns>
    public bool CheckPosition(BlockScript parentObj)
    {
        //Transform���擾
        foreach (Transform Block in parentObj.transform)
        {
            //Transform��؂�グ����int�ɂ���
            Vector2 pos = RoundingScript.Round(Block.position);

            //���Ƀu���b�N�����邩�{�[�h�̓����ɂ��邩
            if (!BoardOutCheck((int)pos.x, (int)pos.y) ||
                BlockCheck((int)pos.x, (int)pos.y))
            {
                //�͈͊O�܂��̓u���b�N����������false��Ԃ�
                return false;
            }
        }
        //�Ȃ�������true��Ԃ�
        return true;
    }
    /// <summary>
    /// �g���ɂ��邩�ǂ������肷��֐�
    /// </summary>
    /// <param name="x">���݂̉��̈ʒu</param>
    /// <param name="y">���݂̏c�̈ʒu</param>
    /// <returns>�͈͓��ɂ��邩</returns>
    private bool BoardOutCheck(int x, int y)
    {
        //���̔���Əc�̔���
        return (x >= 0 && x < _width) && (y >= 0);
    }
    /// <summary>
    /// �ړ���Ƀu���b�N�����邩�ǂ���
    /// </summary>
    /// <param name="x">���݂̉��̈ʒu</param>
    /// <param name="y">���݂̏c�̈ʒu</param>
    /// <param name="block">���݂̃u���b�N</param>
    /// <returns>�ړ���Ƀu���b�N�����邩</returns>
    private bool BlockCheck(int x, int y)
    {
        //�񎟌��z�񂪋󂢂Ă��Ȃ���
        return (_grid[x, y] != 0);
    }
    /// <summary>
    /// �z��Ƀu���b�N���i�[
    /// </summary>
    /// <param name="block">���݂̃u���b�N</param>
    public void SaveBlockInGrid(BlockScript block)
    {
        //�I�u�W�F�N�g�̈ʒu���擾����
        Vector2 pos = new Vector2(0, 0);
        foreach (Transform Item in block.transform)
        {
            //�z��Ɋi�[����
            pos = RoundingScript.Round(Item.position);
            _grid[(int)pos.x, (int)pos.y] = 1;
        }

    }
    /// <summary>
    /// ��������𔻒肵�ď���
    /// </summary>
    public void ClearAllRows()
    {
        //���������������1�i������
        for (int y = 0; y < _height; y++)
        {
            //�������񂪂��邩�ǂ���
            if (LineCheck(y))
            {
                //�������
                LineDelete(y);
                //���������
                //LinesDown(y + 1);
                //y�̒l��߂�
                //y--;
            }
        }
    }
    /// <summary>
    /// �������񂪂��邩�ǂ���
    /// </summary>
    /// <param name="y">�u�����ꏊ�̍���</param>
    /// <returns>bool</returns>
    private bool LineCheck(int y)
    {
        //���̏���܂ŌJ��Ԃ�
        for (int x = 0; x < _width; x++)
        {
            //�P�����ł�0��
            if (_grid[x, y] == 0)
            {
                //0��������false��Ԃ�
                return false;
            }
        }
        //���ׂĖ��܂��Ă���true��Ԃ�
        return true;
    }
    /// <summary>
    /// �������������
    /// </summary>
    /// <param name="y">�u������</param>
    private void LineDelete(int y)
    {
        //�t�B�[���h�ɂ���u���b�N���擾
        _minoBlocks = GameObject.FindGameObjectsWithTag("MinoBlock");
        //��������|�W�V����
        Vector3 checkPsosition = default;
        //�u������̉�����������
        for (int x = 0; x < _width; x++)
        {
            //��������ʒu��������
            checkPsosition = new Vector3(x, y, 0);
            //�I�u�W�F�N�g�̐��J��Ԃ�
            for (int i = 0; i < _minoBlocks.Length; i++)
            {
                //�|�W�V�������������ǂ���
                if (_minoBlocks[i].transform.position == checkPsosition)
                {
                    Debug.Log("in");
                    //�u���b�N������
                    _minoBlocks[i].SetActive(false);
                    //�������ꏊ������������
                    _grid[x, y] = 0;
                    //��̗��������
                    LinesDown(x, y + 1, _minoBlocks[i]);
                    //y�̒l��߂�
                    y--;
                }
            }
        }
        //�X�R�A���Z
        _scoreManager.AddScorePoint();
    }
    /// <summary>
    /// ������ɉ�����
    /// </summary>
    /// <param name="startX">�������u���b�N��X���W</param>
    /// <param name="startY">�������u���b�N��Y���W</param>
    /// <param name="blocks">�������u���b�N</param>
    private void LinesDown(int startX, int startY, GameObject blocks)
    {
        //�u�����ꏊ�������܂ŌJ��Ԃ�
        for (int y = startY; y < _height; y++)
        {
            if (_grid[startX, y] == 0)
            {
                continue;
            }
            _grid[startX, y - 1] = _grid[startX, y];
            _grid[startX, y] = 0;
            for(int i = 0; i < _minoBlocks.Length; i++)
            {
                if (blocks.transform.position.y < _minoBlocks[i].transform.position.y&&blocks.transform.position.x==_minoBlocks[i].transform.position.x)
                {
                    _minoBlocks[i].transform.position += Vector3.down;
                }
            }
        }
        ////�u�����ꏊ�������܂ŌJ��Ԃ�
        //for (int y = startY; y < _height; y++)
        //{
        //    //���̏���܂ŌJ��Ԃ�
        //    for (int x = 0; x < _width; x++)
        //    {
        //        //�Ȃ������玟�̏ꏊ��
        //        if (_grid[x, y] == 0)
        //        {
        //            continue;
        //        }
        //        //�z������ɂ��炷
        //        _grid[x, y - 1] = _grid[x, y];
        //        //���炵���ꏊ��null�ɂ���
        //        _grid[x, y] = 0;
        //        //�I�u�W�F�N�g�����ɓ�����
        //        //_grid[x, y - 1].position += Vector3.down;

        //    }
        //}
    }
    /// <summary>
    /// �Q�[���I�[�o�[���Ă��邩�ǂ���
    /// </summary>
    /// <param name="block"></param>
    /// <returns>bool</returns>
    public bool GameOverLimit(BlockScript block)
    {
        //�u���b�N�̃|�W�V�������擾
        foreach (Transform item in block.transform)
        {
            //�|�W�V��������͈̔͂��z������
            if (item.position.y >= _heightMax)
            {
                //�z������true��Ԃ�
                return true;
            }
        }
        //�z���Ȃ�������false
        return false;
    }
}
