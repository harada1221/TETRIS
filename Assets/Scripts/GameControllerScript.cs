using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [SerializeField, Header("���_���W")]
    private float _positionX = default, _positionY = default;
    [SerializeField, Header("�ǃu���b�N�̃v���n�u")]
    private GameObject _wallBlock = default;
    private int[,] _blockStat = new int[23, 14]; // �e�}�X�̃u���b�N���
    private int[,] _wallBlockPosi = new int[23, 14]{
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,0,0,0,0,0,0,0,0,0,0,1,0,0},
        {1,1,1,1,1,1,1,1,1,1,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0}
    };
    // Start is called before the first frame update
    private void Start()
    {
        _blockStat = _wallBlockPosi;
        // �ǃu���b�N�̔z�u
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                if (_blockStat[j, i] == 1)
                {
                    Instantiate(_wallBlock,new Vector3(i + _positionX, -j + _positionY, 0), Quaternion.identity);
                }
            }
        }
    }
}
