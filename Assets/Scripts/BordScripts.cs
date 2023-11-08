using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BordScripts : MonoBehaviour
{
    [SerializeField]
    private Transform _emptySpriteTransform = default;//マス目のSpriteのTransform
    [SerializeField, Header("マップの高さ")]
    private int _height = 30;
    [SerializeField, Header("マップの横")]
    private int _width = 10;
    [SerializeField, Header("マップの高さ上限")]
    private int _header = 21;
    //スコアをここで管理するのよくない別のクラスに
    private int _score = default;//スコアの数値
    //置いたブロックの周りのブロック数
    private int _blockAroundCheck = default;

   　//Tスピンであるか
    private bool _isTSpin = default;

    [SerializeField, Header("スコアのテキスト")]
    private Text _scoreText = default;
    [SerializeReference, Header("特殊消しを表示するテキスト")]
    private Text _deleteType = default;
    //フィールドの配列
    private Transform[,] _grid = default;
    //初期設定
    private void Awake()
    {
        //配列生成
        _grid = new Transform[_width, _height];
        //枠線を作る
        CreateBoard();
    }
    /// <summary>
    /// ボードを作る
    /// </summary>
    private void CreateBoard()
    {
        //nullだったら処理しない
        if (!_emptySpriteTransform)
        {
            return;
        }
        //高さと横の制限まで作る
        for (int y = 0; y < _header; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                //emptySpriteを生成
                Transform clone = Instantiate(_emptySpriteTransform, new Vector3(x, y, 0), Quaternion.identity);
                clone.transform.parent = transform;
            }
        }

    }
    /// <summary>
    /// 衝突判定をする
    /// </summary>
    /// <param name="parentObj"></param>
    /// <returns>衝突している</returns>
    public bool CheckPosition(BlockScript parentObj)
    {
        //Transformを取得
        foreach (Transform Block in parentObj.transform)
        {
            //Transformを切り上げしてintにする
            Vector2 pos = RoundingScript.Round(Block.position);

            //下にブロックがあるかつボードの外部部にある
            if (!BoardOutCheck((int)pos.x, (int)pos.y) || BlockCheck((int)pos.x, (int)pos.y, parentObj))
            {
                //falseを返す
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 枠内にあるかどうか判定する関数
    /// </summary>
    /// <returns>bool</returns>
    private bool BoardOutCheck(int x, int y)
    {
        //横の判定と縦の判定
        return (x >= 0 && x < _width) && (y >= 0);
    }
    /// <summary>
    /// 移動先にブロックがあるかどうか
    /// </summary>
    /// <returns>bool</returns>
    private bool BlockCheck(int x, int y, BlockScript block)
    {
        //二次元配列が空いていない時かつ親が違うブロックの時
        return (_grid[x, y] != null && _grid[x, y].parent != block.transform);
    }
    /// <summary>
    /// 配列にブロックを格納
    /// </summary>
    /// <param name="block">現在のブロック</param>
    public void SaveBlockInGrid(BlockScript block)
    {
        //オブジェクトの位置を取得する
        Vector2 pos = new Vector2(0, 0);
        foreach (Transform Item in block.transform)
        {
            pos = RoundingScript.Round(Item.position);
            _grid[(int)pos.x, (int)pos.y] = Item;
        }

    }
    /// <summary>
    /// TSpinであるか判定する
    /// </summary>
    /// <param name="block">現在のブロック</param>
    public void TSpinCheck(BlockScript block)
    {
        //TSpinであるか
        _isTSpin = false;
        //オブジェクトの位置を取得する
        Vector2 pos = new Vector2(0, 0);
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
                //ブロックがあればカウントアップ
                if (_grid[(int)pos.x + 1, (int)pos.y - 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            if (BoardOutCheck((int)pos.x - 1, (int)pos.y + 1))
            {
                //ブロックがあればカウントアップ
                if (_grid[(int)pos.x - 1, (int)pos.y + 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            if (BoardOutCheck((int)pos.x - 1, (int)pos.y - 1))
            {
                //ブロックがあればカウントアップ
                if (_grid[(int)pos.x - 1, (int)pos.y - 1] != null)
                {
                    _blockAroundCheck++;
                }
            }
            //ブロックの周りが3つ以上だったらTスピン
            if (_blockAroundCheck >= 3)
            {
                _isTSpin = true;
            }
        }
    }
    /// <summary>
    /// 揃った列を判定して消す
    /// </summary>
    public void ClearAllRows()
    {
        //消した列をカウント
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
                LinesDown(y + 1);
                //消えた列をカウントアップ
                DeleteCount++;
                //yの値を戻す
                y--;
            }
        }
        //TSpinの時消えた列にの数で表示してスコアアップ
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
    /// <param name="y">置いた場所の高さ</param>
    /// <returns>1列揃ってる</returns>
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
    /// <param name="y">置いた列</param>
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
    /// <param name="y">置いたブロックの位置</param>
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
                //配列を下にずらす
                _grid[x, y - 1] = _grid[x, y];
                //ずらした場所をnullにする
                _grid[x, y] = null;
                //オブジェクトを下に動かす
                _grid[x, y - 1].position += Vector3.down;

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
            if (T.position.y >= _header)
            {
                return true;
            }
        }
        return false;
    }
}
