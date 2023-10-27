using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField, Header("生成するブロック")]
    private BlockScript[] _blocks = default;
    [SerializeReference, Header("表示するブロック")]
    private GameObject[] _lookBlock = default;
    [SerializeField]
    private int[] _blooksLen = default;
    private int _radomNum = default;
    private GameObject[] _saveBloocks = default;


    private void Awake()
    {
        _radomNum = Random.RandomRange(0, _blooksLen.Length);
        _saveBloocks = new GameObject[3];
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
            Debug.Log(_radomNum+"RRR");
            Debug.Log(_blooksLen[_radomNum]);
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

        if (block)
        {
            return block;
        }
        else
        {
            return null;
        }
    }
    public GameObject[] LookBlock()
    {
        int i = _blooksLen[_radomNum];
        for (int j = 0; j < _saveBloocks.Length; j++)
        {
            if (_saveBloocks[j] != null)
            {
                Destroy(_saveBloocks[j].gameObject);
            }
        }
        Debug.Log(i + "IIII");
        //1つ先のブロックを表示させる
        _saveBloocks[0] = Instantiate(_lookBlock[_blooksLen[i]], transform.position + new Vector3(10, -5, 0), Quaternion.identity);

        _saveBloocks[1] = Instantiate(_lookBlock[_blooksLen[i]], transform.position + new Vector3(10, -9, 0), Quaternion.identity);

        _saveBloocks[2] = Instantiate(_lookBlock[_blooksLen[i]], transform.position + new Vector3(10, -13, 0), Quaternion.identity);
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
