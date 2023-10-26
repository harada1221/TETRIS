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
    /// <summary>
    /// ブロックが枠内にあるかまたは下にブロックがあるかどうか
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
    private bool BlockCheck(int x, int y, BlockScript block)
    {
        //二次元配列が空いていない時と親が違うブロックの時
        return (_grid[x, y] != null && _grid[x, y].parent != block.transform);
    }
    /// <summary>
    /// 配列にブロックを格納
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
    /// 揃った列を判定して消す
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
    /// 揃った列があるかどうか
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
    /// 揃った列を消す
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
                Debug.Log("何もないよ");

                Destroy(_grid[x, y].gameObject);

            }
            _grid[x, y] = null;
        }
    }
    /// <summary>
    /// 列が消えたら一段下げる
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
    /// ゲームオーバーしているかどうか
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
