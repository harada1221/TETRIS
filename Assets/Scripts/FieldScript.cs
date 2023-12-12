//--------------------------------------
//作成日10/20
//作成者原田
//
//フィールドの管理
//--------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScripts : MonoBehaviour
{
    //フィールドの高さ
    private const int _height = 30;
    //フィールドの横
    private int _width = 10;
    //フィールドの高さ上限
    private int _heightMax = 21;
    //スコア管理のスクリプト
    private ScoreManagerScript _scoreManager = default;
    //フィールドの配列
    private int[,] _grid = default;
    //ブロックの配列
    private GameObject[] _minoBlocks = default;
    /// <summary>
    /// 初期設定
    /// </summary>
    private void Awake()
    {
        //配列生成
        _grid = new int[_width, _height];
        //ScoreManagerScriptを取得
        _scoreManager = GameObject.FindObjectOfType<ScoreManagerScript>();
    }
    /// <summary>
    /// 衝突判定をする
    /// </summary>
    /// <param name="parentObj">現在のブロック</param>
    /// <returns>ぶつかっているか</returns>
    public bool CheckPosition(BlockScript parentObj)
    {
        //Transformを取得
        foreach (Transform Block in parentObj.transform)
        {
            //Transformを切り上げしてintにする
            Vector2 pos = RoundingScript.Round(Block.position);

            //下にブロックがあるかつボードの内部にあるか
            if (!BoardOutCheck((int)pos.x, (int)pos.y) ||
                BlockCheck((int)pos.x, (int)pos.y))
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
    /// <param name="x">現在の横の位置</param>
    /// <param name="y">現在の縦の位置</param>
    /// <returns>範囲内にあるか</returns>
    private bool BoardOutCheck(int x, int y)
    {
        //横の判定と縦の判定
        return (x >= 0 && x < _width) && (y >= 0);
    }
    /// <summary>
    /// 移動先にブロックがあるかどうか
    /// </summary>
    /// <param name="x">現在の横の位置</param>
    /// <param name="y">現在の縦の位置</param>
    /// <param name="block">現在のブロック</param>
    /// <returns>移動先にブロックがあるか</returns>
    private bool BlockCheck(int x, int y)
    {
        //二次元配列が空いていない時
        return (_grid[x, y] != 0);
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
            _grid[(int)pos.x, (int)pos.y] = 1;
            Debug.Log(_grid[(int)pos.x, (int)pos.y] + "+++" + (int)pos.x + "+++" + (int)pos.y);
        }

    }
    /// <summary>
    /// 揃った列を判定して消す
    /// </summary>
    public void ClearAllRows()
    {
        //高さの上限まで繰り返す
        for (int y = 0; y < _height; y++)
        {
            //揃った列があるかどうか
            if (LineCheck(y))
            {
                //列を消す
                LineDelete(y);
                ////列を下げる
                //LinesDown(y + 1);
                ////yの値を戻す
                //y--;
            }
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
            //１か所でも0か
            if (_grid[x, y] == 0)
            {
                //0だったらfalseを返す
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
        //フィールドにあるブロックを取得
        _minoBlocks = GameObject.FindGameObjectsWithTag("MinoBlock");
        //検索するポジション
        Vector3 checkPsosition = default;
        //置いた列の横を検索する
        for (int x = 0; x < _width; x++)
        {
            //検索する位置を代入する
            checkPsosition = new Vector3(x, y, 0);
            //オブジェクトの数繰り返す
            for (int i = 0; i < _minoBlocks.Length; i++)
            {
                //ポジションが同じかどうか
                if ((int)_minoBlocks[i].transform.position.x == (int)checkPsosition.x && (int)_minoBlocks[i].transform.position.y == (int)checkPsosition.y && _minoBlocks[i].activeInHierarchy)
                {
                    //ブロックを消す
                    _minoBlocks[i].SetActive(false);
                    Debug.Log(_minoBlocks[i].transform.position);
                    //消した場所を初期化する
                    _grid[x, y] = 0;
                    LinesDown(checkPsosition);
                }
            }
        }
        //スコア加算
        _scoreManager.AddScorePoint();
    }
    /// <summary>
    /// 列を下に下げる
    /// </summary>
    private void LinesDown(Vector3 startPosition)
    {
        float x = default;
        float y = default;
        for (int i = 0; i < _minoBlocks.Length; i++)
        {
            x = _minoBlocks[i].transform.position.x;
            y = _minoBlocks[i].transform.position.y;
            if ((int)x == (int)startPosition.x && (int)y > (int)startPosition.y && _minoBlocks[i].activeInHierarchy)
            {
                if (_grid[(int)x, (int)y] == 0)
                {
                    continue;
                }
                Debug.Log((int)x + "+++" + (int)y + _minoBlocks[i].gameObject.name);
                //配列を下にずらす
                _grid[(int)x, (int)y - 1] = _grid[(int)x, (int)y];
                //ずらした場所をnullにする
                _grid[(int)x, (int)y] = 0;
                //オブジェクトを下に動かす
                //_minoBlocks[i].transform.position += Vector3.down;
            }
        }
        ////置いた場所から上限まで繰り返す
        //for (int y = startY; y < _height; y++)
        //{
        //    //横の上限まで繰り返す
        //    for (int x = 0; x < _width; x++)
        //    {
        //        //なかったら次の場所に
        //        if (_grid[x, y] == 0)
        //        {
        //            continue;
        //        }
        //        //配列を下にずらす
        //        _grid[x, y - 1] = _grid[x, y];
        //        //ずらした場所をnullにする
        //        _grid[x, y] = 0;
        //        //オブジェクトを下に動かす
        //        //_grid[x, y - 1].position += Vector3.down;

        //    }
        //}
    }
    /// <summary>
    /// ゲームオーバーしているかどうか
    /// </summary>
    /// <param name="block"></param>
    /// <returns>bool</returns>
    public bool GameOverLimit(BlockScript block)
    {
        //ブロックのポジションを取得
        foreach (Transform item in block.transform)
        {
            //ポジションが上の範囲を越えたか
            if (item.position.y >= _heightMax)
            {
                //越えたらtrueを返す
                return true;
            }
        }
        //越えなかったらfalse
        return false;
    }
}
