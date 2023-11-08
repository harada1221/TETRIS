using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField, Header("生成するブロック")]
    private BlockScript[] _blocks = default;
    [SerializeField,Header("ブロックが出てくる順番")]
    private int[] _blooksLen = default;
    //_blooksLenのインデックス値
    private int _lenPoint = default;
    //ネクストを表示させたい数
    private BlockScript[] _saveBloocks = new BlockScript[3];

    //ミノの位置調整
    private Vector3 _minoBlockRevision = new Vector3(0.5f, 0.5f, 0);

    //ネクストミノを表示させる位置
    private Vector3 _nestFastPosition = new Vector3(10, -6, 0);
    private Vector3 _nestSecondPosition = new Vector3(10, -10, 0);
    private Vector3 _nestThirdPosition = new Vector3(10, -14, 0);


    private void Awake()
    {
        //最初の値だけランダムで決める
        _lenPoint = Random.Range(0, _blooksLen.Length);
    }
    /// <summary>
    /// ブロックを選ぶ
    /// </summary>
    /// <returns></returns>
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
    /// 選ばれたブロックを生成
    /// </summary>
    /// <returns></returns>
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
    /// ネクストのブロックを表示
    /// </summary>
    /// <returns></returns>
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
        //1つ先のブロックを表示させる
        int i = 0;
        //配列を超える場合最初に戻る
        if (_lenPoint + 1 < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + 1];
        }
        //1番目ブロックを生成
        _saveBloocks[0] = Instantiate(_blocks[i], transform.position + _nestFastPosition, Quaternion.identity);
        //配列を超える場合最初に戻る
        if (_lenPoint + 2 < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + 2];
        }
        //2つ先を表示
        else
        {
            i = _lenPoint + 2 - _blooksLen.Length;
        }
        //2番目ブロックを生成
        _saveBloocks[1] = Instantiate(_blocks[i], transform.position + _nestSecondPosition, Quaternion.identity);

        //配列を超える場合最初に戻る
        if (_lenPoint + 3 < _blooksLen.Length)
        {
            i = _blooksLen[_lenPoint + 3];
        }
        //3つ先を表示
        else
        {
            i = _lenPoint + 3 - _blooksLen.Length;
        }
        //3番目ブロックを生成
        _saveBloocks[2] = Instantiate(_blocks[i], transform.position + _nestThirdPosition, Quaternion.identity);

        //すべて入ってたらreturnする
        if (_saveBloocks[0] != null && _saveBloocks[1] != null && _saveBloocks[2] != null)
        {
            return _saveBloocks;
        }
        else
        {
            return null;
        }
    }
}
