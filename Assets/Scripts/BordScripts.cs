using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordScripts : MonoBehaviour
{
    [SerializeField]
    private Transform _emptySprite = default;
    [SerializeField, Header("マップの広さ")]
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
    /// ボードを作る
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
    //ブロックが枠内にあるかまたは下にブロックがあるかどうか
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
    /// 枠内にあるかどうか判定する関数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool BoardOutCheck(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0);
    }
    /// <summary>
    /// 移動先にブロックがあるかどうか
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    private bool BlockCheck(int x, int y,BlockScript block)
    {
        //二次元配列が空いていない時と親が違うブロックの時
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
