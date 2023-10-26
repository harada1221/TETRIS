using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordScripts : MonoBehaviour
{
    [SerializeField]
    private Transform _emptySprite = default;
    [SerializeField, Header("�}�b�v�̍L��")]
    private int _height = 30, _width = 10, _header = 8;

    private Transform[,] _grid = default;

    private void Awake()
    {
        _grid = new Transform[_width, _height];
    }
    private void Start()
    {
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
        foreach (Transform B in block.transform)
        {
            Vector2 pos = RoundingScript.Round(B.position);

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
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool BoardOutCheck(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0);
    }
    /// <summary>
    /// �ړ���Ƀu���b�N�����邩�ǂ���
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="block"></param>
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
        foreach (Transform Item in block.transform)
        {
            Vector2 pos = RoundingScript.Round(Item.position);
            _grid[(int)pos.x, (int)pos.y] = Item;
        }
    }
    /// <summary>
    /// ��������𔻒肵�ď���
    /// </summary>
    public void ClearAllRows()
    {
        for (int y = 0; y < _height; y++)
        {
            if (IsComplete(y))
            {
                //ClearRow(y);

                ShitRowsDown(y + 1);
                ClearRow(y);
                y--;
            }

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
            Debug.Log(x + "XXX");
            Debug.Log(y + "YYY");
            Debug.Log(_grid[x, y]);
            if (_grid[x, y]! == null)
            {
                Debug.Log("�����Ȃ���");

                Destroy(_grid[x, y].gameObject);

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
                    _grid[x, y - 1] = _grid[x, y];
                    _grid[x, y] = null;
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
            if (T.transform.position.y >= _height - _header)
            {
                return true;
            }
        }
        return false;
    }
}
