using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField, Header("生成するブロック")]
    private BlockScript[] _blocks = default;
    [SerializeField, Header("ブロックが出てくる順番")]
    private int[] _blooksLen = default;
    //_blooksLenのインデックス値
    private int _lenPoint = default;
    //ネクストを表示させたい数
    private BlockScript[] _saveBloocks = new BlockScript[3];

    //Iミノの位置調整
    private Vector3 _minoBlockRevision = new Vector3(0.5f,0.5f, 0);

    //ネクストミノを表示させるそれぞれの位置
    private Vector3 _nestFastPosition = new Vector3(10, -6, 0);
    private Vector3 _nestSecondPosition = new Vector3(10, -10, 0);
    private Vector3 _nestThirdPosition = new Vector3(10, -14, 0);

    //ネクストミノが何番先か加算する値
    private const int _fastIndex = 1;
    private const int _secondIndex = 2;
    private const int _thirdIndex = 3;
    //ネクストブロックを保存する配列の値
    private int _saveIndex = 0;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        //最初の値だけランダムで決める
        _lenPoint = Random.Range(0, _blooksLen.Length);
    }
    /// <summary>
    /// <para>ブロックを選ぶ</para>
    /// </summary>
    /// <returns>BlockScript</returns>
    private BlockScript GetRandomBlock()
    {
        //中身が入っているか
        if (_blocks[_blooksLen[_lenPoint]])
        {
            //lenPointを1づつ増やす
            _lenPoint++;
            //配列の最後になったら最初に戻す
            if (_lenPoint > _blooksLen.Length - 1)
            {
                _lenPoint = 0;
            }
            //生成するブロック渡す
            return _blocks[_blooksLen[_lenPoint]];
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// <para>選ばれたブロックを生成</para>
    /// </summary>
    /// <returns>BlockScript</returns>
    public BlockScript SpwnBlock()
    {
        //選ばれたブロックを生成
        BlockScript block = Instantiate(GetRandomBlock(), transform.position, Quaternion.identity);
        //Iミノブロックだけ位置調整
        if (block.GetISpin)
        {
            block.transform.position += _minoBlockRevision;
        }
        //ネクストを更新する
        NextBlock();

        //生成されていたら渡す
        if (block)
        {
            return block;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// <para>ネクストのブロックを表示する</para>
    /// </summary>
    /// <returns>BlockScript[]</returns>
    private BlockScript[] NextBlock()
    {
        //ネクストを消す
        for (int j = 0; j < _saveBloocks.Length; j++)
        {
            if (_saveBloocks[j] != null)
            {
                Destroy(_saveBloocks[j].gameObject);
            }
        }
        //1つ先のブロックを決定するindex値
        int i = 0;
        //配列を超える場合最初に戻る
        if (_lenPoint + _fastIndex < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + _fastIndex];
        }
        //ブロックのホールドを保存する値を初期化
        _saveIndex = 0;
        //1番目ブロックを生成
        _saveBloocks[_saveIndex] = Instantiate(_blocks[i], transform.position + _nestFastPosition, Quaternion.identity);

        //カウントアップ
        _saveIndex++;

        //配列を超える場合最初に戻る
        if (_lenPoint + _secondIndex < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + _secondIndex];
        }
        //2つ先を表示
        else
        {
            i = _lenPoint + _secondIndex - _blooksLen.Length;
        }
        //2番目ブロックを生成
        _saveBloocks[_saveIndex] = Instantiate(_blocks[i], transform.position + _nestSecondPosition, Quaternion.identity);

        //カウントアップ
        _saveIndex++;

        //配列を超える場合最初に戻る
        if (_lenPoint + _thirdIndex < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + _thirdIndex];
        }
        //3つ先を表示
        else
        {
            i = _lenPoint + _thirdIndex - _blooksLen.Length;
        }
        //3番目ブロックを生成
        _saveBloocks[_saveIndex] = Instantiate(_blocks[i], transform.position + _nestThirdPosition, Quaternion.identity);

        //カウントアップ
        _saveIndex++;

        //配列がすべて入っているか確認する
        for (int K = 0; K < _saveIndex; K++)
        {
            //入ってたら次の値に
            if (_saveBloocks[K] != null)
            {
                continue;
            }
            else
            {
                //なかったらnullを返す
                return null;
            }
        }
        //BlockScriptの配列を渡す
        return _saveBloocks;
    }
}
