using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScripts : MonoBehaviour
{
    [SerializeField, Header("マス目のSpriteのTransform")]
    private Transform _emptySpriteTransform = default;
    [SerializeField, Header("フィールドの高さ")]
    private int _height = 30;
    [SerializeField, Header("フィールドの横")]
    private int _width = 10;
    [SerializeField, Header("フィールドの高さ上限")]
    private int _header = 21;
    //置いたブロックの周りのブロック数
    private int _blockAroundCheck = default;
   　//Tスピンであるか
    private bool _isTSpin = default;
    //スコア管理のスクリプト
    private ScoreManagerScript _scoreManager = default;
    //フィールドの配列
    private Transform[,] _grid = default;
    /// <summary>
    /// 初期設定
    /// </summary>
    private void Awake()
    {
        //配列生成
        _grid = new Transform[_width, _height];
        //枠線を作る
        CreateBoard();
        //ScoreManagerScriptを取得
        _scoreManager = GameObject.FindObjectOfType<ScoreManagerScript>();
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
        //高さの上限まで繰り返す
        for (int y = 0; y < _header; y++)
        {
            //横の上限まで繰り返す
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
    /// <returns>bool</returns>
    public bool CheckPosition(BlockScript parentObj)
    {
        //Transformを取得
        foreach (Transform Block in parentObj.transform)
        {
            //Transformを切り上げしてintにする
            Vector2 pos = RoundingScript.Round(Block.position);

            //下にブロックがあるかつボードの外部部にあるか
            if (!BoardOutCheck((int)pos.x, (int)pos.y) || BlockCheck((int)pos.x, (int)pos.y, parentObj))
            {
                //範囲外またはブロックがあったらfalseを返す
                return false;
            }
        }
        //なかったらtrueを返す
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
        return (_grid[x, y] != null) && (_grid[x, y].parent != block.transform);
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
            //配列に格納する
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
            //周りのブロックの数
            _blockAroundCheck = 0;
            pos = block.transform.position;
            //枠内にあるかどうか検索する
            //あったらカウントアップ
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
            if (LineCheck(y))
            {
                //列を消す
                LineDelete(y);
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
            //Tスピンを表示
            _scoreManager.TspinDisplay(DeleteCount);
            //スコア加算
            _scoreManager.AddScorePoint();
        }
        //4列以上消えたらテトリスを表示
        else if (DeleteCount >= 4)
        {
            //テトリス表示
            _scoreManager.TetrisDisply();
            //スコア加算
            _scoreManager.AddScorePoint();
        }
        else
        {
            //何もなかったら表示しない
            _scoreManager.TextClear();
        }
    }
    /// <summary>
    /// 揃った列があるかどうか
    /// </summary>
    /// <param name="y">置いた場所の高さ</param>
    /// <returns>bool</returns>
    private bool LineCheck(int y)
    {
        //横の上限まで繰り返す
        for (int x = 0; x < _width; x++)
        {
            //１か所でもnullか
            if (_grid[x, y] == null)
            {
                //nullだったらfalseを返す
                return false;
            }
        }
        //すべて埋まってたらtrueを返す
        return true;
    }
    /// <summary>
    /// 揃った列を消す
    /// </summary>
    /// <param name="y">置いた列</param>
    private void LineDelete(int y)
    {
        //置いた列の横を検索する
        for (int x = 0; x < _width; x++)
        {
            //ポジションにオブジェクトがあるか
            if (_grid[x, y] != null)
            {
                //ポジションにあるオブジェクトを消す
                Destroy(_grid[x, y].gameObject);
            }
            //消した場所を初期化する
            _grid[x, y] = null;
        }
        //スコア加算
        _scoreManager.AddScorePoint();
    }
    /// <summary>
    /// 列が消えたら一段下げる
    /// </summary>
    /// <param name="y">置いたブロックの上の位置</param>
    private void LinesDown(int startY)
    {
        //置いた場所から上限まで繰り返す
        for (int y = startY; y < _height; y++)
        {
            //横の上限まで繰り返す
            for (int x = 0; x < _width; x++)
            {
                //なかったら次の場所に
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
    /// <returns>bool</returns>
    public bool GameOverLimit(BlockScript block)
    {
        //ブロックのポジションを取得
        foreach (Transform T in block.transform)
        {
            //ポジションが上の範囲を越えたか
            if (T.position.y >= _header)
            {
                //越えたらtrueを返す
                return true;
            }
        }
        //越えなかったらfalse
        return false;
    }
}
