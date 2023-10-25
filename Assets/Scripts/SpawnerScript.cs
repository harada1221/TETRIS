using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField,Header("��������u���b�N")]
    private BlockScript[] _blocks = default;
    /// <summary>
    /// �������_���u���b�N��I��
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
    /// �I�΂ꂽ�u���b�N�𐶐�
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
