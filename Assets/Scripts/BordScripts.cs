using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BordScripts : MonoBehaviour
{
    [SerializeField]
    private Transform _emptySprite = default;
    [SerializeField, Header("�}�b�v�̍L��")]
    private int _height = 30, _width = 10, _header = 8;
    private int _score = default;//�X�R�A�̐��l
    private int _blockAroundCheck = default;//�u�����u���b�N�̎���̊m�F

    private bool isTSpin = default; //T�~�m�u���b�N�ł��邩

    [SerializeField, Header("�X�R�A�̃e�L�X�g")]
    private Text _scoreText = default;
    [SerializeReference, Header("�������̎��")]
    private Text _deleteType;

    private Transform[,] _grid = default;//�t�B�[���h�̑傫��
    private void Awake()
    {
        //�z�񐶐�
        _grid = new Transform[_width, _height];
    }

    private void Start()
    {
        //�g�������
        CreateBoard();
    }
    /// <summary>
    /// �{�[�h�����
    /// </summary>
    private void CreateBoard()
    {
        if (_emptySprite)
        {
            for (int y = 0; y < _height - _header; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Transform clone = Instantiate(_emptySprite, new Vector3(x, y, 0), Quaternion.identity);
                    clone.transform.parent = transform;
                }
            }
        }
    }
    /// <summary>
    /// �u���b�N���g���ɂ��邩�܂��͉��Ƀu���b�N�����邩�ǂ���
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool CheckPosition(BlockScript block)
    {
        //Transform���擾
        foreach (Transform B in block.transform)
        {
            //Transform��؂�グ����int�ɂ���
            Vector2 pos = RoundingScript.Round(B.position);

            //���Ƀu���b�N����������false��Ԃ�
            if (!BoardOutCheck((int)pos.x, (int)pos.y) || BlockCheck((int)pos.x, (int)pos.y, block))
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// �g���ɂ��邩�ǂ������肷��֐�
    /// </summary>
    /// <returns></returns>
    private bool BoardOutCheck(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0);
    }
    /// <summary>
    /// �ړ���Ƀu���b�N�����邩�ǂ���
    /// </summary>
    /// <returns></returns>
    private bool BlockCheck(int x, int y, BlockScript block)
    {
        //�񎟌��z�񂪋󂢂Ă��Ȃ����Ɛe���Ⴄ�u���b�N�̎�
        return (_grid[x, y] != null && _grid[x, y].parent != block.transform);
    }
    /// <summary>
    /// �z��Ƀu���b�N���i�[
    /// </summary>
    /// <param name="block"></param>
    public void SaveBlockInGrid(BlockScript block)
    {
        isTSpin = false;
        Vector2 pos = new Vector2(0, 0);
        foreach (Transform Item in block.transform)
        {
            pos = RoundingScript.Round(Item.position);
            _grid[(int)pos.x, (int)pos.y] = Item;
        }
        //T�X�s�����ǂ����u�����ꏊ�̎�����m�F
        if (block.GetTspin == true)
        {
            _blockAroundCheck = 0;
            pos = block.transform.position;
            //�g���ɂ��邩�ǂ���
            if (BoardOutCheck((int)pos.x + 1, (int)pos.y + 1))
            {
                //�u���b�N������΃J�E���g�A�b�v
                if (_grid[(int)pos.x + 1, (int)pos.y + 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            if (BoardOutCheck((int)pos.x + 1, (int)pos.y - 1))
            {
                if (_grid[(int)pos.x + 1, (int)pos.y - 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            if (BoardOutCheck((int)pos.x - 1, (int)pos.y + 1))
            {
                if (_grid[(int)pos.x - 1, (int)pos.y + 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            if (BoardOutCheck((int)pos.x - 1, (int)pos.y - 1))
            {
                if (_grid[(int)pos.x - 1, (int)pos.y - 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            //�u���b�N�̎��肪3�ȏゾ������T�X�s��
            if (_blockAroundCheck >= 3)
            {
                isTSpin = true;
            }
        }
    }
    /// <summary>
    /// ��������𔻒肵�ď���
    /// </summary>
    public void ClearAllRows()
    {
        int DeleteCount = 0;

        //���������������1�i������
        for (int y = 0; y < _height; y++)
        {
            //�������񂪂��邩�ǂ���
            if (IsComplete(y))
            {
                //�������
                ClearRow(y);
                //���������
                ShitRowsDown(y + 1);
                //����������J�E���g�A�b�v
                DeleteCount++;
                y--;
            }
        }
        //TSpin�̎���������ɂ̐��ŕ\�����ăX�R�A�A�b�v
        if (isTSpin)
        {
            switch (DeleteCount)
            {
                case 1:
                    _deleteType.text = "T-Spin Single";
                    _score += 1000;
                    break;
                case 2:
                    _deleteType.text = "T-Spin Double";
                    _score += 2000;
                    break;
                case 3:
                    _deleteType.text = "T-Spin Triple";
                    _score += 3000;
                    break;
            }
        }
        //4��ȏ��������e�g���X��\��
        else if (DeleteCount >= 4)
        {
            _deleteType.text = "TETRIS";
            _score += 1000;
            _scoreText.text = "Score:" + _score.ToString();
        }
        else
        {
            //�����Ȃ�������\�����Ȃ�
            _deleteType.text = "";
        }
    }
    /// <summary>
    /// �������񂪂��邩�ǂ���
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IsComplete(int y)
    {
        for (int x = 0; x < _width; x++)
        {
            //�P�����ł����炾������false��Ԃ�
            if (_grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// �������������
    /// </summary>
    /// <param name="y"></param>
    private void ClearRow(int y)
    {
        for (int x = 0; x < _width; x++)
        {
            if (_grid[x, y] != null)
            {
                //�|�W�V�����ɂ���I�u�W�F�N�g������
                Destroy(_grid[x, y].gameObject);
                //�X�R�A���Z
                _score += 10;
                _scoreText.text = "Score:" + _score.ToString();
            }
            _grid[x, y] = null;
        }
    }
    /// <summary>
    /// �񂪏��������i������
    /// </summary>
    /// <param name="y"></param>
    private void ShitRowsDown(int startY)
    {
        for (int y = startY; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_grid[x, y] != null)
                {
                    //�z������ɂ��炷
                    _grid[x, y - 1] = _grid[x, y];
                    //���炵���ꏊ��null�ɂ���
                    _grid[x, y] = null;
                    //�I�u�W�F�N�g�����ɓ�����
                    _grid[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }
    }
    /// <summary>
    /// �Q�[���I�[�o�[���Ă��邩�ǂ���
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool OverLimit(BlockScript block)
    {
        foreach (Transform T in block.transform)
        {
            //�|�W�V��������͈̔͂��z������true��Ԃ�
            if (T.transform.position.y >= _height - _header - 1)
            {
                return true;
            }
        }
        return false;
    }
}
