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
    //�u���b�N���g���ɂ��邩�܂��͉��Ƀu���b�N�����邩�ǂ���
    public bool CheckPosition(BlockScript block)
    {
        foreach (Transform B in block.transform)
        {
            Vector2 pos = RoundingScript.Round(B.position);

            if (!BoardOutCheck((int)pos.x, (int)pos.y)|| BlockCheck((int)pos.x, (int)pos.y, block))
            {
                return false;
            }
        }
        return false;
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
    private bool BlockCheck(int x, int y,BlockScript block)
    {
        //�񎟌��z�񂪋󂢂Ă��Ȃ����Ɛe���Ⴄ�u���b�N�̎�
        return (_grid[x,y] != null && _grid[x,y].parent != block.transform);
    }

    public void SaveBlockInGrid(BlockScript block)
    {
        foreach(Transform Item in block.transform)
        {
            Vector2 pos = RoundingScript.Round(Item.position);
            _grid[(int)pos.x, (int)pos.y] = Item;
        }
    }
}
