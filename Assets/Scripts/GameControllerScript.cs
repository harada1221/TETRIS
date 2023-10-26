// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
// 作成日10月17日:
// 編集日10月18日:
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
    [SerializeField, Header("落下ブロックの初期X座標")]
    private int _fallBlockInitPosX = default; // 落下ブロックの初期位置:X座標
    [SerializeField, Header("落下ブロックの初期Y座標")]
    private int _fallBlockInitPosY = default; // 落下ブロックの初期位置:Y座標
    private int _fallBlockPosX = default; // 落下ブロックのX座標
    private int _fallBlockPosY = default; // 落下ブロックのY座標
    private int[,] _fallBlockStat = new int[4, 4]; // 落下ブロック状態
    private int _blockNum; // ブロック種類
    private int[,] _blockStat = new int[23, 14]; // 各マスのブロック状態
    private MinoScript _minoScript = default;//ミノのスクリプト

    private bool isdown = false;//落下フラグ
    private bool isHitRightWall = false;//右壁のフラグ
    private bool isHitLeftWall = false;//右壁のフラグ
   // private float _groundCountTime = default; // 落下ブロックが着地してからの経過時間
    public bool GetisDown { get => isdown; }
    public bool GetRightWall { get => isHitRightWall; }
    public bool GetLeftWall { get => isHitLeftWall; }

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

    private GameStat _gameStatus = GameStat.START;
    public enum GameStat // ゲームステータス
    {
        GAMEOVER,
        START,
        GROUND,
        ERASE
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

        //ミノをランダム生成
        _blockNum = Random.Range(0, 7);
        _minoScript = _minoBlock[5].GetComponent<MinoScript>();
        _fallBlockStat = _fallBlockSet.set(_minoScript);

        Instantiate(_minoBlock[5], new Vector3(_fallBlockPosX + _positionX, -_fallBlockPosY + _positionY, 0), Quaternion.identity);
    }
    private void Update()
    {
        //for (int i = 0; i < 4; i++)
        //{
        //    for (int j = 0; j < 4; j++)
        //    {
        //        Destroy(_fallBlockObj[j, i]);
        //        if (_fallBlockStat[j, i] == 2)
        //        {
        //            _fallBlockObj[j, i] = Instantiate(_minoBlock[0], new Vector3(_fallBlockPosX + i + _positionX, -_fallBlockPosY - j + _positionY, 0), Quaternion.identity);
        //        }
        //    }
        //}
        judgeContactRight(_blockStat, _fallBlockPosX, _fallBlockPosY);
        judgeContactLeft(_blockStat, _fallBlockPosX, _fallBlockPosY);
        switch (_gameStatus)
        {
            case GameStat.START:
                // 落下ブロックの着地判定
                if (judgeGround(_blockStat, _fallBlockPosX, _fallBlockPosY) == true)
                {
                    _gameStatus = GameStat.GROUND;
                }
                break;
        }
    }
    /// <summary>
    /// ミノが着地したか
    /// </summary>
    public bool judgeGround(int[,] blockStat, int x, int y)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 3; j >= 0; j--)
            {
                if (_fallBlockStat[i, j] == 2)
                {
                    if (blockStat[y + j + 1, x + i] == 1 || blockStat[y + j, x + i + 1] == 3)
                    {
                        isdown = true;
                        break;
                    }
                    else
                    {
                        isdown = false;
                    }
                }
            }
        }
        return isdown;
    }
    /// <summary>
    /// 右の壁判定
    /// </summary>
    public bool judgeContactRight(int[,] blockStat, int x, int y)
    {
        for (int j = 0; j < 4; j++)
        {
            for (int i = 3; i >= 0; i--)
            {
                if (_fallBlockStat[j, i] == 2)
                {
                    if (blockStat[y + j, x + i + 1] == 1 || blockStat[y + j, x + i + 1] == 3)
                    {
                        Debug.Log("右壁");
                        isHitRightWall = true;
                        break;
                    }
                    else
                    {
                        isHitRightWall = false;
                    }
                }
            }
        }
        return isHitRightWall;
    }
    /// <summary>
    /// 左の壁判定
    /// </summary>
    public bool judgeContactLeft(int[,] blockStat, int x, int y)
    {
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (_fallBlockStat[j, i] == 2)
                {
                    if (blockStat[y + j, x + i - 1] == 1 || blockStat[y + j, x + i - 1] == 3)
                    {
                        Debug.Log("左壁");
                        isHitLeftWall = true;
                        break;
                    }
                    else
                    {
                        isHitLeftWall = false;
                    }
                }
            }
        }
        return isHitLeftWall;
    }
    public void DownYPosition()
    {
        _fallBlockPosY++;
    }
    public void RightXPosition()
    {
        _fallBlockPosX++;
    }
    public void LeftXPosition()
    {
        _fallBlockPosX--;
    }
}
