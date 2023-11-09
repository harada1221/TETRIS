using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScripts : MonoBehaviour
{
    [SerializeField, Header("�}�X�ڂ�Sprite��Transform")]
    private Transform _emptySpriteTransform = default;
    [SerializeField, Header("�t�B�[���h�̍���")]
    private int _height = 30;
    [SerializeField, Header("�t�B�[���h�̉�")]
    private int _width = 10;
    [SerializeField, Header("�t�B�[���h�̍������")]
    private int _header = 21;
    //�u�����u���b�N�̎���̃u���b�N��
    private int _blockAroundCheck = default;
   �@//T�X�s���ł��邩
    private bool _isTSpin = default;
    //�X�R�A�Ǘ��̃X�N���v�g
    private ScoreManagerScript _scoreManager = default;
    //�t�B�[���h�̔z��
    private Transform[,] _grid = default;
    /// <summary>
    /// �����ݒ�
    /// </summary>
    private void Awake()
    {
        //�z�񐶐�
        _grid = new Transform[_width, _height];
        //�g�������
        CreateBoard();
        //ScoreManagerScript���擾
        _scoreManager = GameObject.FindObjectOfType<ScoreManagerScript>();
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
        //�����̏���܂ŌJ��Ԃ�
        for (int y = 0; y < _header; y++)
        {
            //���̏���܂ŌJ��Ԃ�
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
    /// <returns>bool</returns>
    public bool CheckPosition(BlockScript parentObj)
    {
        //Transform���擾
        foreach (Transform Block in parentObj.transform)
        {
            //Transform��؂�グ����int�ɂ���
            Vector2 pos = RoundingScript.Round(Block.position);

            //���Ƀu���b�N�����邩�{�[�h�̊O�����ɂ��邩
            if (!BoardOutCheck((int)pos.x, (int)pos.y) || BlockCheck((int)pos.x, (int)pos.y, parentObj))
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
        return (_grid[x, y] != null) && (_grid[x, y].parent != block.transform);
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
            //����̃u���b�N�̐�
            _blockAroundCheck = 0;
            pos = block.transform.position;
            //�g���ɂ��邩�ǂ�����������
            //��������J�E���g�A�b�v
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
            if (LineCheck(y))
            {
                //�������
                LineDelete(y);
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
            //T�X�s����\��
            _scoreManager.TspinDisplay(DeleteCount);
            //�X�R�A���Z
            _scoreManager.AddScorePoint();
        }
        //4��ȏ��������e�g���X��\��
        else if (DeleteCount >= 4)
        {
            //�e�g���X�\��
            _scoreManager.TetrisDisply();
            //�X�R�A���Z
            _scoreManager.AddScorePoint();
        }
        else
        {
            //�����Ȃ�������\�����Ȃ�
            _scoreManager.TextClear();
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
            //�P�����ł�null��
            if (_grid[x, y] == null)
            {
                //null��������false��Ԃ�
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
        //�u������̉�����������
        for (int x = 0; x < _width; x++)
        {
            //�|�W�V�����ɃI�u�W�F�N�g�����邩
            if (_grid[x, y] != null)
            {
                //�|�W�V�����ɂ���I�u�W�F�N�g������
                Destroy(_grid[x, y].gameObject);
            }
            //�������ꏊ������������
            _grid[x, y] = null;
        }
        //�X�R�A���Z
        _scoreManager.AddScorePoint();
    }
    /// <summary>
    /// �񂪏��������i������
    /// </summary>
    /// <param name="y">�u�����u���b�N�̏�̈ʒu</param>
    private void LinesDown(int startY)
    {
        //�u�����ꏊ�������܂ŌJ��Ԃ�
        for (int y = startY; y < _height; y++)
        {
            //���̏���܂ŌJ��Ԃ�
            for (int x = 0; x < _width; x++)
            {
                //�Ȃ������玟�̏ꏊ��
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
    /// <returns>bool</returns>
    public bool GameOverLimit(BlockScript block)
    {
        //�u���b�N�̃|�W�V�������擾
        foreach (Transform T in block.transform)
        {
            //�|�W�V��������͈̔͂��z������
            if (T.position.y >= _header)
            {
                //�z������true��Ԃ�
                return true;
            }
        }
        //�z���Ȃ�������false
        return false;
    }
}
