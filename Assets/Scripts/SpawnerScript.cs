using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField, Header("生成するブロック")]
    private BlockScript[] _blocks = default;
    [SerializeField]
    private int[] _blooksLen = default;
    private int _radomNum = default;
    private BlockScript[] _saveBloocks = default;


    private void Awake()
    {
        _radomNum = Random.RandomRange(0, _blooksLen.Length);
        _saveBloocks = new BlockScript[3];
    }
    /// <summary>
    /// ラランダムブロックを選ぶ
    /// </summary>
    /// <returns></returns>
    private BlockScript GetRandomBlock()
    {
        if (_blocks[_blooksLen[_radomNum]])
        {
            _radomNum++;
            if (_radomNum > _blooksLen.Length - 1)
            {
                _radomNum = 0;
            }
            return _blocks[_blooksLen[_radomNum]];
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
        BlockScript block = Instantiate(GetRandomBlock(), transform.position, Quaternion.identity);
        LookBlock();

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
    private BlockScript[] LookBlock()
    {
        for (int j = 0; j < _saveBloocks.Length; j++)
        {
            if (_saveBloocks[j] != null)
            {
                Destroy(_saveBloocks[j].gameObject);
            }
        }
        //1つ先のブロックを表示させる
        int i = 0;
        if (_radomNum + 1 < _blooksLen.Length)
        {
            i = _blooksLen[_radomNum + 1];
        }
        _saveBloocks[0] = Instantiate(_blocks[i], transform.position + new Vector3(10, -6, 0), Quaternion.identity);
        if (_radomNum + 2 < _blooksLen.Length)
        {
            i = _blooksLen[_radomNum + 2];
        }
        else
        {
            i = _radomNum + 2 - _blooksLen.Length;
        }
        _saveBloocks[1] = Instantiate(_blocks[i], transform.position + new Vector3(10, -10, 0), Quaternion.identity);
        if (_radomNum + 3 < _blooksLen.Length)
        {
            i = _blooksLen[_radomNum + 3];
        }
        else
        {
            i = _radomNum + 3 - _blooksLen.Length;
        }
        _saveBloocks[2] = Instantiate(_blocks[i], transform.position + new Vector3(10, -14, 0), Quaternion.identity);

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
