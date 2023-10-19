// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
//作成日10月17日:
// 作成日10月18日:
// 作成者:原田
// ---------------------------------------------------------using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [SerializeField, Header("原点座標")]
    private float _positionX = default, _positionY = default;
    private GameObject[,] _blockObj = new GameObject[21, 14]; // 表示ブロックオブジェクト
    private GameObject[,] _fallBlockObj = new GameObject[4, 4]; // 落下ブロックオブジェクト
    [SerializeField, Header("壁ブロックのプレハブ")]
    private GameObject _wallBlock = default;
    [SerializeField, Header("ブロックのPrehab")]
    private GameObject[] _minoBlock = default;
    private FallBlockSet _fallBlockSet; // FallBlockSetクラス
    [SerializeField]
    private int _fallBlockInitPosX = default; // 落下ブロックの初期位置:X座標
    [SerializeField]
    private int _fallBlockInitPosY = default; // 落下ブロックの初期位置:Y座標
    private int _fallBlockPosX = default; // 落下ブロックのX座標
    private int _fallBlockPosY = default; // 落下ブロックのY座標
    private int[,] fallBlockStat = new int[4, 4]; // 落下ブロック状態

    private int _blockNum; // ブロック種類
    private int _rot; // ブロック回転状態
    private int[,] _blockStat = new int[23, 14]; // 各マスのブロック状態
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
        _fallBlockSet = new FallBlockSet(); // FallBlockSetクラスのインスタンス生成
        _blockStat = _wallBlockPosi;
        // 壁ブロックの配置
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
        // 落下ブロック読み込み
        _fallBlockPosX = _fallBlockInitPosX;
        _fallBlockPosY = _fallBlockInitPosY;

        _blockNum = Random.Range(0, 7);
        _rot = 0;
        fallBlockStat = _fallBlockSet.set(_blockNum, _rot);
        Instantiate(_minoBlock[_blockNum], new Vector3(_fallBlockPosX + _positionX, -_fallBlockPosY + _positionY, 0), Quaternion.identity);
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
}
