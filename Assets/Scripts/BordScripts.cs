using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BordScripts : MonoBehaviour
{
    [SerializeField]
    private Transform _emptySpriteTransform = default;//�}�X�ڂ�Sprite��Transform
    [SerializeField, Header("�}�b�v�̍���")]
    private int _height = 30;
    [SerializeField, Header("�}�b�v�̉�")]
    private int _width = 10;
    [SerializeField, Header("�}�b�v�̍������")]
    private int _header = 21;
    //�X�R�A�������ŊǗ�����̂悭�Ȃ��ʂ̃N���X��
    private int _score = default;//�X�R�A�̐��l
    //�u�����u���b�N�̎���̃u���b�N��
    private int _blockAroundCheck = default;

   �@//T�X�s���ł��邩
    private bool _isTSpin = default;

    [SerializeField, Header("�X�R�A�̃e�L�X�g")]
    private Text _scoreText = default;
    [SerializeReference, Header("���������\������e�L�X�g")]
    private Text _deleteType = default;
    //�t�B�[���h�̔z��
    private Transform[,] _grid = default;
    //�����ݒ�
    private void Awake()
    {
        //�z�񐶐�
        _grid = new Transform[_width, _height];
        //�g�������
        CreateBoard();
    }
    /// <summary>
    /// �{�[�h�����
    /// </summary>
    private void CreateBoard()
    {
        //null�������珈�����Ȃ�
        if (!_emptySpriteTransform)
        {
            return;
        }
        //�����Ɖ��̐����܂ō��
        for (int y = 0; y < _header; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                //emptySprite�𐶐�
                Transform clone = Instantiate(_emptySpriteTransform, new Vector3(x, y, 0), Quaternion.identity);
                clone.transform.parent = transform;
            }
        }

    }
    /// <summary>
    /// �Փ˔��������
    /// </summary>
    /// <param name="parentObj"></param>
    /// <returns>�Փ˂��Ă���</returns>
    public bool CheckPosition(BlockScript parentObj)
    {
        //Transform���擾
        foreach (Transform Block in parentObj.transform)
        {
            //Transform��؂�グ����int�ɂ���
            Vector2 pos = RoundingScript.Round(Block.position);

            //���Ƀu���b�N�����邩�{�[�h�̊O�����ɂ���
            if (!BoardOutCheck((int)pos.x, (int)pos.y) || BlockCheck((int)pos.x, (int)pos.y, parentObj))
            {
                //false��Ԃ�
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// �g���ɂ��邩�ǂ������肷��֐�
    /// </summary>
    /// <returns>bool</returns>
    private bool BoardOutCheck(int x, int y)
    {
        //���̔���Əc�̔���
        return (x >= 0 && x < _width) && (y >= 0);
    }
    /// <summary>
    /// �ړ���Ƀu���b�N�����邩�ǂ���
    /// </summary>
    /// <returns>bool</returns>
    private bool BlockCheck(int x, int y, BlockScript block)
    {
        //�񎟌��z�񂪋󂢂Ă��Ȃ������e���Ⴄ�u���b�N�̎�
        return (_grid[x, y] != null && _grid[x, y].parent != block.transform);
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
            pos = RoundingScript.Round(Item.position);
            _grid[(int)pos.x, (int)pos.y] = Item;
        }

    }
    /// <summary>
    /// TSpin�ł��邩���肷��
    /// </summary>
    /// <param name="block">���݂̃u���b�N</param>
    public void TSpinCheck(BlockScript block)
    {
        //TSpin�ł��邩
        _isTSpin = false;
        //�I�u�W�F�N�g�̈ʒu���擾����
        Vector2 pos = new Vector2(0, 0);
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
                //�u���b�N������΃J�E���g�A�b�v
                if (_grid[(int)pos.x + 1, (int)pos.y - 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            if (BoardOutCheck((int)pos.x - 1, (int)pos.y + 1))
            {
                //�u���b�N������΃J�E���g�A�b�v
                if (_grid[(int)pos.x - 1, (int)pos.y + 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            if (BoardOutCheck((int)pos.x - 1, (int)pos.y - 1))
            {
                //�u���b�N������΃J�E���g�A�b�v
                if (_grid[(int)pos.x - 1, (int)pos.y - 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            //�u���b�N�̎��肪3�ȏゾ������T�X�s��
            if (_blockAroundCheck >= 3)
            {
                _isTSpin = true;
            }
        }
    }
    /// <summary>
    /// ��������𔻒肵�ď���
    /// </summary>
    public void ClearAllRows()
    {
        //����������J�E���g
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
                LinesDown(y + 1);
                //����������J�E���g�A�b�v
                DeleteCount++;
                //y�̒l��߂�
                y--;
            }
        }
        //TSpin�̎���������ɂ̐��ŕ\�����ăX�R�A�A�b�v
        if (_isTSpin)
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
    /// <param name="y">�u�����ꏊ�̍���</param>
    /// <returns>1�񑵂��Ă�</returns>
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
    /// <param name="y">�u������</param>
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
    /// <param name="y">�u�����u���b�N�̈ʒu</param>
    private void LinesDown(int startY)
    {
        for (int y = startY; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_grid[x, y] == null)
                {
                    continue;
                }
                //�z������ɂ��炷
                _grid[x, y - 1] = _grid[x, y];
                //���炵���ꏊ��null�ɂ���
                _grid[x, y] = null;
                //�I�u�W�F�N�g�����ɓ�����
                _grid[x, y - 1].position += Vector3.down;

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
            if (T.position.y >= _header)
            {
                return true;
            }
        }
        return false;
    }
}
