using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField,Header("生成するブロック")]
    private BlockScript[] _blocks = default;
    /// <summary>
    /// ラランダムブロックを選ぶ
    /// </summary>
    /// <returns></returns>
    private BlockScript GetRandomBlock()
    {
        int i = Random.RandomRange(0, _blocks.Length);
        if (_blocks[i])
        {
            return _blocks[i];
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
}
