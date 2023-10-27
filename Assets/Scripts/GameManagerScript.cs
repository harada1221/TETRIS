using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private SpawnerScript _spawner = default;//ブロックスポナー
    private BlockScript _activeBlock = default;//生成されたブロック格納
    private BlockScript _holdBlock = default;//ホールドしたブロックを格納
    private BlockScript _saveBlock = default;//ホールドしたブロックを入れ替える用
    [SerializeField, Header("ブロックが落ちる時間")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//次にブロックが落ちるまでの時間

    private BordScripts _bord = default;
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;
    private float _lookTime = default;

    [SerializeField, Header("下入力のインターバル")]
    private float _nextKeyDownTimeInterval = default;
    [SerializeField, Header("左右入力のインターバル")]
    private float _nextKeySideTimeInterval = default;
    [SerializeField, Header("回転入力のインターバル")]
    private float _nextKeyRotateTimeInterval = default;
    [SerializeField, Header("ブロックの固定の時間")]
    private float _lookTimeInterval = default;
    [SerializeField]
    private GameObject _gameOverPanel = default;
    private bool isGameOver = false;
    private bool isChangeBlock = false;
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
        _lookTime = Time.time + _lookTimeInterval;

        //ブロックを生成して格納
        if (!_activeBlock)
        {
            _activeBlock = _spawner.SpwnBlock();
        }
        //アクティブ状態だと消す
        if (_gameOverPanel.activeInHierarchy)
        {
            _gameOverPanel.SetActive(false);
        }
    }
    private void Update()
    {
        if (isGameOver)
        {
            return;
        }
        PlayerInput();
    }
    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            _lookTime = Time.time + _lookTimeInterval;
            //右に動かす
            _activeBlock.MoveRight();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
            }
        }
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            _lookTime = Time.time + _lookTimeInterval;
            //左に動かす
            _activeBlock.MoveLeft();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveRight();
            }
        }
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            _lookTime = Time.time + _lookTimeInterval;
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
            _lookTime = Time.time + _lookTimeInterval;
            //右回転
            _activeBlock.RotateLeft();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateRight();
            }
        }
        else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime) || Time.time > _nextDropTimer)
        {
            //下に動かす
            _activeBlock.MoveDown();

            _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
            _nextDropTimer = Time.time + _dropInterval;

            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {

                if (_bord.OverLimit(_activeBlock))
                {
                    GameOver();
                }
                else
                {

                    //底についた処理
                    BottomBoard();
                }
            }
        }
        //ホールドの処理
        else if (Input.GetKeyDown(KeyCode.Z) && isChangeBlock == false)
        {
            isChangeBlock = true;
            if (_holdBlock == default)
            {
                //1回目の処理
                _holdBlock = Instantiate(_activeBlock, new Vector3(-6, 15, 0), Quaternion.identity);
                Destroy(_activeBlock.gameObject);
                _activeBlock = _spawner.SpwnBlock();
                //タイムの初期化
                _nextDropTimer = Time.time;
                _nextKeySideTime = Time.time;
                _nextKeyRotateTime = Time.time;
                _lookTime = Time.time;
            }
            else
            {
                //入れ替え
                _saveBlock = _activeBlock;
                _activeBlock = _holdBlock;
                _holdBlock = _saveBlock;
               
                Destroy(_saveBlock.gameObject);
                Destroy(_activeBlock.gameObject);
                _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);

                Destroy(_holdBlock.gameObject);

                _holdBlock = Instantiate(_saveBlock, new Vector3(-6, 15, 0), Quaternion.identity);

                _nextDropTimer = Time.time;
                _nextKeySideTime = Time.time;
                _nextKeyRotateTime = Time.time;
                _lookTime = Time.time;
            }
        }
    }
    /// <summary>
    /// 底に着いた時の処理4
    /// </summary>
    private void BottomBoard()
    {
        _activeBlock.MoveUp();
        Debug.Log(_lookTime);
        Debug.Log(Time.time+"TTTT");
        if (Time.time > _lookTime)
        {
            Debug.Log("日あった");
           
            _bord.SaveBlockInGrid(_activeBlock);

            isChangeBlock = false;
            _activeBlock = _spawner.SpwnBlock();

            //タイムの初期化
            _nextDropTimer = Time.time;
            _nextKeySideTime = Time.time;
            _nextKeyRotateTime = Time.time;
            _lookTime = Time.time;
            _bord.ClearAllRows();//揃っていれば削除
        }
    }
    /// <summary>
    /// ゲームオーバーの時呼び出す
    /// </summary>
    private void GameOver()
    {
        _activeBlock.MoveUp();
        //表示する
        if (!_gameOverPanel.activeInHierarchy)
        {
            _gameOverPanel.SetActive(true);
        }

        isGameOver = true;
    }
    /// <summary>
    /// シーンを呼び出す
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
}
