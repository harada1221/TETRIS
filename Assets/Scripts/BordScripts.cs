using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BordScripts : MonoBehaviour
{
    [SerializeField]
    private Transform _emptySprite = default;
    [SerializeField, Header("マップの広さ")]
    private int _height = 30, _width = 10, _header = 8;
    private int _score = default;//スコアの数値
    private int _blockAroundCheck = default;//置いたブロックの周りの確認

    private bool isTSpin = default; //Tミノブロックであるか

    [SerializeField, Header("スコアのテキスト")]
    private Text _scoreText = default;
    [SerializeReference, Header("消し方の種類")]
    private Text _deleteType;

    private Transform[,] _grid = default;//フィールドの大きさ
    private void Awake()
    {
        //配列生成
        _grid = new Transform[_width, _height];
    }

    private void Start()
    {
        //枠線を作る
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
        //Transformを取得
        foreach (Transform B in block.transform)
        {
            //Transformを切り上げしてintにする
            Vector2 pos = RoundingScript.Round(B.position);

            //下にブロックがあったらfalseを返す
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
    /// <returns></returns>
    private bool BoardOutCheck(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0);
    }
    /// <summary>
    /// 移動先にブロックがあるかどうか
    /// </summary>
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
        isTSpin = false;
        Vector2 pos = new Vector2(0, 0);
        foreach (Transform Item in block.transform)
        {
            pos = RoundingScript.Round(Item.position);
            _grid[(int)pos.x, (int)pos.y] = Item;
        }
        //Tスピンかどうか置いた場所の周りを確認
        if (block.GetTspin == true)
        {
            _blockAroundCheck = 0;
            pos = block.transform.position;
            //枠内にあるかどうか
            if (BoardOutCheck((int)pos.x + 1, (int)pos.y + 1))
            {
                //ブロックがあればカウントアップ
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
            //ブロックの周りが3つ以上だったらTスピン
            if (_blockAroundCheck >= 3)
            {
                isTSpin = true;
            }
        }
    }
    /// <summary>
    /// 揃った列を判定して消す
    /// </summary>
    public void ClearAllRows()
    {
        int DeleteCount = 0;

        //揃った列を消して1段下げる
        for (int y = 0; y < _height; y++)
        {
            //揃った列があるかどうか
            if (IsComplete(y))
            {
                //列を消す
                ClearRow(y);
                //列を下げる
                ShitRowsDown(y + 1);
                //消えた列をカウントアップ
                DeleteCount++;
                y--;
            }
        }
        //TSpinの時消えた列にの数で表示してスコアアップ
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
        //4列以上消えたらテトリスを表示
        else if (DeleteCount >= 4)
        {
            _deleteType.text = "TETRIS";
            _score += 1000;
            _scoreText.text = "Score:" + _score.ToString();
        }
        else
        {
            //何もなかったら表示しない
            _deleteType.text = "";
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
            //１か所でもからだったらfalseを返す
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
            if (_grid[x, y] != null)
            {
                //ポジションにあるオブジェクトを消す
                Destroy(_grid[x, y].gameObject);
                //スコア加算
                _score += 10;
                _scoreText.text = "Score:" + _score.ToString();
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
                    //配列を下にずらす
                    _grid[x, y - 1] = _grid[x, y];
                    //ずらした場所をnullにする
                    _grid[x, y] = null;
                    //オブジェクトを下に動かす
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
            //ポジションが上の範囲を越えたらtrueを返す
            if (T.transform.position.y >= _height - _header - 1)
            {
                return true;
            }
        }
        return false;
    }
}
