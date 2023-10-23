// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
//�쐬��10��17��:
// �ҏW��10��18��:
// �쐬��:���c
// ---------------------------------------------------------using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [SerializeField, Header("���_���W")]
    private float _positionX = default, _positionY = default;
    //private GameObject[,] _blockObj = new GameObject[21, 14]; // �\���u���b�N�I�u�W�F�N�g
    //private GameObject[,] _fallBlockObj = new GameObject[4, 4]; // �����u���b�N�I�u�W�F�N�g
    [SerializeField, Header("�ǃu���b�N�̃v���n�u")]
    private GameObject _wallBlock = default;
    [SerializeField, Header("�u���b�N��Prehab")]
    private GameObject[] _minoBlock = default;
    private FallBlockSet _fallBlockSet; // FallBlockSet�N���X
    [SerializeField]
    private int _fallBlockInitPosX = default; // �����u���b�N�̏����ʒu:X���W
    [SerializeField]
    private int _fallBlockInitPosY = default; // �����u���b�N�̏����ʒu:Y���W
    private int _fallBlockPosX = default; // �����u���b�N��X���W
    private int _fallBlockPosY = default; // �����u���b�N��Y���W
    private int[,] _fallBlockStat = new int[4, 4]; // �����u���b�N���
    private MinoScript _minoScript = default;

    private int _blockNum; // �u���b�N���
    private int _rot; // �u���b�N��]���
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
        _fallBlockSet = new FallBlockSet(); // FallBlockSet�N���X�̃C���X�^���X����
        _blockStat = _wallBlockPosi;
        // �ǃu���b�N�̔z�u
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                if (_blockStat[j, i] == 1)
                {
                    Instantiate(_wallBlock, new Vector3(i + _positionX, -j + _positionY, 0), Quaternion.identity);
                }
            }
        }
        _blockStat = _wallBlockPosi;
        // �����u���b�N�ǂݍ���
        _fallBlockPosX = _fallBlockInitPosX;
        _fallBlockPosY = _fallBlockInitPosY;

        _blockNum = Random.Range(0, 7);
        _rot = 0;
        _minoScript = _minoBlock[_blockNum].GetComponent<MinoScript>();
        _fallBlockStat = _fallBlockSet.set(_minoScript);
        Instantiate(_minoBlock[5], new Vector3(_fallBlockPosX + _positionX, -_fallBlockPosY + _positionY, 0), Quaternion.identity);
    }
    //private void Update()
    //{
    //    for (int i = 0; i < 4; i++)
    //    {
    //        for (int j = 0; j < 4; j++)
    //        {
    //            Destroy(_fallBlockObj[j, i]);
    //            if (fallBlockStat[j, i] == 2)
    //            {
    //                _fallBlockObj[j, i] = Instantiate(_minoBlock[0], new Vector3(_fallBlockPosX + i + _positionX, -_fallBlockPosY - j + _positionY, 0), Quaternion.identity);
    //            }
    //        }
    //    }
    //}
    public bool judgeGround(MinoScript minoScript, int rot, int[,] blockStat, int x, int y)
    {
        bool groundFlg = false;
        int[,] block = new int[4, 4];
        block = _fallBlockSet.set(minoScript);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 3; j >= 0; j--)
            {
                if (block[j, i] == 2)
                {
                    if (blockStat[y + j + 1, x + i] == 1 || blockStat[y + j + 1, x + i] == 3)
                    {
                        groundFlg = true;
                        break;
                    }
                }
            }
        }
        return groundFlg;
    }
}
