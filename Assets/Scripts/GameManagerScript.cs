using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField, Header("ゴーストブロックの色")]
    private Color _colorWhite = default;
    private Vector3 _ghostBlockPosition = new Vector3(0, 0, 0.5f);

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
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
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
        //ゲームオーバーだったら動かさない
        if (isGameOver)
        {
            return;
        }
        //ゴーストブロックを下まで落とす
        DownGhostBlock();
        //プレイヤーの操作
        PlayerInput();
    }
    private void PlayerInput()
    {
        //右に移動
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //動かせる回数をカウントアップ
            _moveCount++;
            //着地から固定までの時間
            _lookTime = Time.time + _lookTimeInterval;
            //右に動かす
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
            //タイマーリセット
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
                _ghostBlock.MoveLeft();
            }
        }
        //左に移動
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //動かせる回数をカウントアップ
            _moveCount++;
            //着地から固定までの時間
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
        //右回転
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //動かせる回数をカウントアップ
            _moveCount++;
            //着地から固定までの時間
            _lookTime = Time.time + _lookTimeInterval;
            //右回転
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                //回転補正OとI以外
                if (_activeBlock.GetSuperspin)
                {
                    TRotationRight();
                }
                if (_activeBlock.GetISpin)
                {
                    IRotationRight();
                }
            }
        }
        //左回転
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //着地してから動ける回数
            _moveCount++;
            //着地から固定までの時間
            _lookTime = Time.time + _lookTimeInterval;
            //左回転
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                if (_activeBlock.GetSuperspin)
                {
                    TRotationLeft();
                }
                if (_activeBlock.GetISpin)
                {
                    IRotationLeft();
                }
            }
        }
        //ソフトドロップ
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
            //ホールドを１度だけにする
            isChangeBlock = true;
            if (_holdBlock == default)
            {
                //1回目の処理
                _holdBlock = Instantiate(_activeBlock, new Vector3(-6, 15, 0), Quaternion.identity);
                //ブロック削除
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                //ブロック生成
                _activeBlock = _spawner.SpwnBlock();
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
                //色を変える
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

                //元のブロックを削除
                Destroy(_saveBlock.gameObject);
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                //新しくブロックを生成
                _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);
                //Iミノだったら位置調整
                if (_activeBlock.GetISpin)
                {
                    _activeBlock.transform.position += new Vector3(0.5f, 0.5f, 0);
                }
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
                //色を変える
                ColorChange();
                //ホールドブロックを削除
                Destroy(_holdBlock.gameObject);
                //表示ようホールドブロック生成
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
            //配列に格納
            _bord.SaveBlockInGrid(_activeBlock);
            //ホールドをできるように
            isChangeBlock = false;
            //ブロックを消して次のブロックを生成
            Destroy(_ghostBlock.gameObject);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
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
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// ぶつかるまで下におとす
    /// </summary>
    private void DownGhostBlock()
    {
        _ghostBlock.transform.position = _activeBlock.transform.position + _ghostBlockPosition;
        while (_bord.CheckPosition(_ghostBlock))
        {
            //下に動かす
            _ghostBlock.MoveDown();
        }
        _ghostBlock.MoveUp();
    }
    /// <summary>
    /// オブジェクトの色を変える
    /// </summary>
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
    /// <summary>
    /// 左回転の回転補正
    /// </summary>
    private void TRotationLeft()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:

                _activeBlock.transform.position += new Vector3(1, 0, 0);

                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, -2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 90:
                _activeBlock.transform.position += new Vector3(1, 0, 0);

                if ((!_bord.CheckPosition(_activeBlock)))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if ((!_bord.CheckPosition(_activeBlock)))
                {
                    _activeBlock.transform.position += new Vector3(-1, -3, 0);
                }
                if ((!_bord.CheckPosition(_activeBlock)))
                {
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 270:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 180:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, -2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
        }
    }
    /// <summary>
    /// 右回転の回転補正
    /// </summary>
    private void TRotationRight()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 90:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 270:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 180:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
        }

    }
    /// <summary>
    /// Iミノブロックの左回転補正
    /// </summary>
    private void IRotationLeft()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:
                _activeBlock.transform.position += new Vector3(2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 90:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 2, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-2, 1, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 180:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, -2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 270:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, -1, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
        }
    }
    /// <summary>
    /// Iミノブロックの右回転補正
    /// </summary>
    private void IRotationRight()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:
                _activeBlock.transform.position += new Vector3(-2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -2, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;

            case 90:
                _activeBlock.transform.position += new Vector3(2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 180:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 2, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 270:
                _activeBlock.transform.position += new Vector3(-2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-2, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
        }

    }
}
