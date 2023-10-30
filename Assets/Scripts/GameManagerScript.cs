using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField,Header("ゴーストブロックの色")]
    private Color _colorWhite = default;

    private SpawnerScript _spawner = default;//ブロックスポナー
    private BlockScript _activeBlock = default;//生成されたブロック格納
    private BlockScript _ghostBlock = default;//生成されたゴーストブロック
    private BlockScript _holdBlock = default;//ホールドしたブロックを格納
    private BlockScript _saveBlock = default;//ホールドしたブロックを入れ替える用
    [SerializeField, Header("ブロックが落ちる時間")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//次にブロックが落ちるまでの時間

    private BordScripts _bord = default;//フィールドのスクリプト
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;
    private float _lookTime = default;

    [SerializeField, Header("ブロックを動かした回数")]
    private int _moveCount = 15;

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
    private bool isGameOver = false;//ゲームオーバーの判定
    private bool isChangeBlock = false;//ホールドそをしたか
    private bool isGround = false;

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
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            ColorChange();
            while (_bord.CheckPosition(_ghostBlock))
            {
                //下に動かす
                _ghostBlock.MoveDown();
            }
            _ghostBlock.MoveUp();
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
        DownGhostBlock();
        PlayerInput();
    }
    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            _moveCount++;
            _lookTime = Time.time + _lookTimeInterval;
            //右に動かす
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
                _ghostBlock.MoveLeft();
            }
        }
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            _moveCount++;
            _lookTime = Time.time + _lookTimeInterval;
            //左に動かす
            _activeBlock.MoveLeft();
            _ghostBlock.MoveLeft();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveRight();
                _ghostBlock.MoveRight();
            }
        }
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            _moveCount++;
            _lookTime = Time.time + _lookTimeInterval;
            //右回転
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateLeft();
                _ghostBlock.RotateLeft();
            }
        }
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            _moveCount++;
            _lookTime = Time.time + _lookTimeInterval;
            //右回転
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateRight();
                _ghostBlock.RotateRight();
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
                //範囲外だとゲームオーバー
                if (_bord.OverLimit(_activeBlock))
                {
                    GameOver();
                }
                else
                {
                    if (!isGround)
                    {
                        isGround = true;
                        _lookTime = Time.time + _lookTimeInterval;
                    }
                    //底についた処理
                    BottomBoard();
                }
            }
        }
        //ハードドロップ
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //ぶつかるまで繰り返す
            while (_bord.CheckPosition(_activeBlock))
            {
                //下に動かす
                _activeBlock.MoveDown();
            }

            BottomBoard();

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
                Destroy(_ghostBlock.gameObject);
                _activeBlock = _spawner.SpwnBlock();
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
                ColorChange();
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

                //元からブロックを削除してインスタンスする
                Destroy(_saveBlock.gameObject);
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
                ColorChange();
                Destroy(_holdBlock.gameObject);

                _holdBlock = Instantiate(_saveBlock, new Vector3(-6, 15, 0), Quaternion.identity);

                //タイムの初期化
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
        if (Time.time > _lookTime || Input.GetKeyDown(KeyCode.W) || _moveCount >= 15)
        {
            _bord.SaveBlockInGrid(_activeBlock);

            isChangeBlock = false;
            Destroy(_ghostBlock.gameObject);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            ColorChange();
            //値の初期化
            _nextDropTimer = Time.time;
            _nextKeySideTime = Time.time;
            _nextKeyRotateTime = Time.time;
            _lookTime = Time.time;
            _moveCount = 0;
            isGround = false;
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
    private void DownGhostBlock()
    {
        _ghostBlock.transform.position = _activeBlock.transform.position;
        while (_bord.CheckPosition(_ghostBlock))
        {
            //下に動かす
            _ghostBlock.MoveDown();
        }
        _ghostBlock.MoveUp();
    }
    private void ColorChange()
    {
        Transform parent = _ghostBlock.transform;
        SpriteRenderer chidren;

        // 子オブジェクトを全て取得する
        foreach (Transform child in parent)
        {
            chidren = child.GetComponent<SpriteRenderer>();
            chidren.color = _colorWhite;
        }
    }
}
