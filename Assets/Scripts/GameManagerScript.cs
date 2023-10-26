using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private SpawnerScript _spawner;//ブロックスポナー
    private BlockScript _activeBlock;//生成されたブロック格納
    [SerializeField, Header("ブロックが落ちる時間")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//次にブロックが落ちるまでの時間

    private BordScripts _bord = default;
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;

    [SerializeField, Header("下入力のインターバル")]
    private float _nextKeyDownTimeInterval = default;
    [SerializeField, Header("左右入力のインターバル")]
    private float _nextKeySideTimeInterval = default;
    [SerializeField, Header("回転入力のインターバル")]
    private float _nextKeyRotateTimeInterval = default;
    private void Start()
    {
        //スポナーオブジェクトを格納
        _spawner = GameObject.FindObjectOfType<SpawnerScript>();
        //ボードのスクリプトを格納
        _bord = GameObject.FindObjectOfType<BordScripts>();

        //タイマーの初期設定
        _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
        _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
        _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;

        //ブロックを生成して格納
        if (!_activeBlock)
        {
            _activeBlock = _spawner.SpwnBlock();
        }
    }
    private void Update()
    {
        PlayerInput();
        if (Time.time > _nextDropTimer)
        {
            _nextDropTimer = Time.time + _dropInterval;
            if (_activeBlock)
            {
                //下に動かす
                _activeBlock.MoveDown();

                //枠からはみ出していないか
                if (!_bord.CheckPosition(_activeBlock))
                {
                    //上に戻す
                    _activeBlock.MoveUp();
                    //配列に格納
                    _bord.SaveBlockInGrid(_activeBlock);
                    //ブロック生成
                    _activeBlock = _spawner.SpwnBlock();
                }
            }
        }
    }
    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //右に動かす
            _activeBlock.MoveRight();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
            }
        }
        else if(Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //左に動かす
            _activeBlock.MoveLeft();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveRight();
            }
        }
        else if(Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //右回転
            _activeBlock.RotateRight();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateLeft();
            }
        }
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //右回転
            _activeBlock.RotateLeft();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateRight();
            }
        }
        //else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime)/* || Time.time > _nextKeyDownTime*/)
        //{
        //    //下に動かす
        //    _activeBlock.MoveDown();

        //    _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
        //    _nextDropTimer = Time.time + _dropInterval;
            
        //    //はみ出してたら戻す
        //    if (!_bord.CheckPosition(_activeBlock))
        //    {
                
        //    }
        //}
    }
}
